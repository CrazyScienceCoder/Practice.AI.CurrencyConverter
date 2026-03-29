process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";

/** @type {import('next').NextConfig} */
const nextConfig = {
  output: "standalone",
  experimental: {
    serverComponentsExternalPackages: [],
  },
};

export default nextConfig;
