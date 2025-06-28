import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";

// https://vite.dev/config/
export default defineConfig({
	plugins: [vue()],
	server: {
		host: "local.dev",
		port: 5000,
		strictPort: true,
		cors: false
	}
});
