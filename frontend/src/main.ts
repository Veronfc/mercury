import "./style.css";
import App from "./App.vue";
import NotFoundView from "./views/NotFoundView.vue";
import HomeView from "./views/HomeView.vue";
import AuthView from "./views/AuthView.vue";
import ConversationView from "./views/ConversationView.vue";
import UserView from "./views/UserView.vue";

const routes = [
	{ path: "/:pathMatch(.*)*", name: "404", component: NotFoundView },
	{ path: "/", name: "home", component: HomeView },
	{ path: "/auth", name: "auth", component: AuthView },
	{ path: "/user", name: "user", component: UserView },
	{
		path: "/conversations/:id?",
		name: "conversations",
		component: ConversationView
	}
];

const router = createRouter({
	history: createWebHistory(),
	routes
});

const pinia = createPinia();
pinia.use(piniaPluginPersistedState);

const app = createApp(App);
app.use(router);
app.use(pinia);

const userStore = useUserStore();
const { setInfo } = userStore;
const { isLoggedIn } = storeToRefs(userStore);
const { getConversations } = useConversationStore();
await setInfo();

if (isLoggedIn.value) {
	await connectSignalR();
	await getConversations();
}

app.mount("#app");
