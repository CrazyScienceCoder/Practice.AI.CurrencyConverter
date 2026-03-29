import NextAuth, { NextAuthOptions } from "next-auth";
import { JWT } from "next-auth/jwt";

declare module "next-auth" {
  interface Session {
    accessToken?: string;
    error?: string;
  }
}

declare module "next-auth/jwt" {
  interface JWT {
    accessToken?: string;
    refreshToken?: string;
    expiresAt?: number;
    error?: string;
  }
}

async function refreshAccessToken(token: JWT): Promise<JWT> {
  try {
    const issuer = process.env.KEYCLOAK_ISSUER!;
    const url = `${issuer}/protocol/openid-connect/token`;

    const response = await fetch(url, {
      method: "POST",
      headers: { "Content-Type": "application/x-www-form-urlencoded" },
      body: new URLSearchParams({
        client_id: process.env.KEYCLOAK_CLIENT_ID!,
        client_secret: process.env.KEYCLOAK_CLIENT_SECRET!,
        grant_type: "refresh_token",
        refresh_token: token.refreshToken!,
      }),
    });

    const refreshed = await response.json();

    if (!response.ok) {
      console.error("[NextAuth] Token refresh failed:", refreshed);
      throw refreshed;
    }

    return {
      ...token,
      accessToken: refreshed.access_token,
      refreshToken: refreshed.refresh_token ?? token.refreshToken,
      expiresAt: Math.floor(Date.now() / 1000) + refreshed.expires_in,
      error: undefined,
    };
  } catch (error) {
    console.error("[NextAuth] RefreshAccessTokenError:", error);
    return { ...token, error: "RefreshAccessTokenError" };
  }
}

export const authOptions: NextAuthOptions = {
  providers: [
    {
      // Custom provider — bypasses OIDC discovery (which would return Docker-internal
      // 'keycloak:8080' URLs). Each endpoint is set explicitly so we can use
      // localhost:8080 for the browser-facing authorization URL while keeping
      // keycloak:8080 for server-to-server calls (token, userinfo).
      // idToken validation is intentionally omitted; the userinfo endpoint is used instead.
      id: "keycloak",
      name: "Keycloak",
      type: "oauth",
      clientId: process.env.KEYCLOAK_CLIENT_ID!,
      clientSecret: process.env.KEYCLOAK_CLIENT_SECRET!,
      // Must match the 'iss' in Keycloak's id_token (Keycloak uses the request hostname,
      // so the browser-facing URL is what it reports).
      issuer: process.env.KEYCLOAK_ISSUER_EXTERNAL!,
      authorization: {
        url: `${process.env.KEYCLOAK_ISSUER_EXTERNAL}/protocol/openid-connect/auth`,
        params: { scope: "openid email profile" },
      },
      token: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/token`,
      userinfo: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/userinfo`,
      // JWKS URI for ID-token signature verification (server-to-server, Docker-internal URL).
      jwks_endpoint: `${process.env.KEYCLOAK_ISSUER}/protocol/openid-connect/certs`,
      // Use OIDC callback so openid-client validates the id_token Keycloak returns.
      idToken: true,
      checks: ["pkce", "state"],
      profile(profile: Record<string, string>) {
        return {
          id: profile.sub,
          name: profile.name ?? profile.preferred_username,
          email: profile.email,
          image: profile.picture,
        };
      },
    },
  ],
  callbacks: {
    async jwt({ token, account }) {
      // Initial sign-in: persist access + refresh tokens
      if (account) {
        return {
          ...token,
          accessToken: account.access_token,
          refreshToken: account.refresh_token,
          expiresAt: account.expires_at,
        };
      }

      // Return token if not yet expired (with 60s buffer)
      if (Date.now() < (token.expiresAt! - 60) * 1000) {
        return token;
      }

      // Token has expired — refresh it
      return refreshAccessToken(token);
    },
    async session({ session, token }) {
      session.accessToken = token.accessToken;
      session.error = token.error;
      return session;
    },
  },
  session: {
    maxAge: 7 * 24 * 60 * 60, // 7 days — matches Keycloak ssoSessionMaxLifespan
  },
  pages: {
    signIn: "/login",
  },
};

export default NextAuth(authOptions);
