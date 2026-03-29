import { NextResponse } from "next/server";

export async function GET(request: Request) {
  const origin = new URL(request.url).origin;

  // Use the external (browser-facing) issuer for the logout redirect.
  // Falls back to KEYCLOAK_ISSUER when KEYCLOAK_ISSUER_EXTERNAL is not set.
  const issuer =
    process.env.KEYCLOAK_ISSUER_EXTERNAL ?? process.env.KEYCLOAK_ISSUER;
  const clientId = process.env.KEYCLOAK_CLIENT_ID;
  const postLogoutRedirectUri = `${process.env.NEXTAUTH_URL ?? origin}/login?logged_out=true`;

  const keycloakLogoutUrl = new URL(
    `${issuer}/protocol/openid-connect/logout`
  );
  keycloakLogoutUrl.searchParams.set("client_id", clientId!);
  keycloakLogoutUrl.searchParams.set(
    "post_logout_redirect_uri",
    postLogoutRedirectUri
  );

  return NextResponse.redirect(keycloakLogoutUrl.toString());
}
