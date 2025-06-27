<script setup lang="ts">
	import LoginForm from "../components/LoginForm.vue";
	import RegisterForm from "../components/RegisterForm.vue";
	import { useRoute, useRouter } from "vue-router";
	import { useUserStore } from "../stores/useUserStore";

	const userStore = useUserStore();
	const router = useRouter();

	if (userStore.isLoggedIn) {
		router.replace({ name: "home" });
	}

	const route = useRoute();
	const mode = route.query.mode;

	if (!mode || !["register", "login"].includes(mode as string)) {
		router.replace({ name: "home" });
	}
</script>

<template>
	<div>This is the auth view</div>
	<RouterLink to="/">Home</RouterLink>
	<RegisterForm v-if="$route.query.mode === 'register'" />
	<LoginForm v-if="$route.query.mode === 'login'" />
</template>

<style scoped></style>
