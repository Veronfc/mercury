import { createApp } from "vue";
import "./style.css";
import App from "./App.vue";
import NotFoundView from "./views/NotFoundView.vue";
import HomeView from "./views/HomeView.vue";
import AuthView from "./views/AuthView.vue";
import { createRouter, createWebHistory } from "vue-router";
import { createPinia } from "pinia";

const routes = [
	{ path: "/:pathMatch(.*)*", name: "404", component: NotFoundView },
	{ path: "/", name: "home", component: HomeView },
	{ path: "/auth", name: "auth", component: AuthView }
];

const router = createRouter({
	history: createWebHistory(),
	routes
});

const pinia = createPinia();

export const backendUrl = "https://localhost:7177";

createApp(App).use(router).use(pinia).mount("#app");
