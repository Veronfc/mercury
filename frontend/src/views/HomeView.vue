<script setup lang="ts">
	import { useFetch } from "@vueuse/core";
	import { useUserStore } from "../stores/useUserStore";
	import { useRouter } from "vue-router";

	const userStore = useUserStore();
	const router = useRouter();

	const logOut = async () => {
		await useFetch("api/user/logout", {
			credentials: "include"
		}).get();

		userStore.setLoggedIn(false);
		userStore.setInfo();

		router.go(0);
	};
</script>

<template>
	<div>This is the home view</div>
	<span>Logged in: {{ userStore.isLoggedIn }}</span>
	<span v-if="userStore.isLoggedIn">User info: {{ userStore.userInfo }}</span>
	<button @click="logOut">Logout</button>
	<RouterLink :to="{ name: 'auth', query: { mode: 'login' } }">Auth</RouterLink>
</template>

<style scoped></style>
