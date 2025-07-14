import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import tailwindcss from "@tailwindcss/vite";
import AutoImport from "unplugin-auto-import/vite";
import Components from "unplugin-vue-components/vite";
import { VueUseComponentsResolver } from "unplugin-vue-components/resolvers";

// https://vite.dev/config/
export default defineConfig({
	plugins: [
		vue(),
		tailwindcss(),
		AutoImport({
			dts: "src/auto-imports.d.ts",
			dirs: ["src", "src/stores", "src/schemas"],
			imports: [
				{
					vue: ["ref", "computed", "watch", "createApp", "onBeforeMount"]
				},
				{
					"vue-router": [
						"useRouter",
						"useRoute",
						"createRouter",
						"createWebHistory"
					]
				},
				{
					pinia: ["defineStore", "storeToRefs", "createPinia"]
				},
				{
					"@vueuse/core": ["useFetch"]
				},
				{
					"vee-validate": ["useField", "useForm"]
				},
				{
					"@vee-validate/zod": ["toTypedSchema"]
				},
				{
					zod: [["*", "zod"]]
				}
			]
		}),
		Components({
			dts: "src/components.d.ts",
			resolvers: [VueUseComponentsResolver()],
			dirs: ["src/components", "src/views"]
		})
	],
	server: {
		host: "local.dev",
		port: 5000,
		strictPort: true,
		cors: false
	}
});
