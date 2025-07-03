<script setup lang="ts">
	import { useFetch } from "@vueuse/core";
	import { useUserStore } from "../stores/userStore";
	import { useRouter } from "vue-router";
	import { storeToRefs } from "pinia";

	const router = useRouter();
	const userStore = useUserStore();
	const { setInfo } = userStore;
	const { isLoggedIn, userInfo } = storeToRefs(userStore);

	const logOut = async () => {
		await useFetch("api/user/logout", {
			credentials: "include"
		}).get();

		setInfo();
		router.go(0);
	};
</script>

<template>
	<div>This is the home view</div>
	<span>Logged in: {{ isLoggedIn }}</span>
	<span v-if="isLoggedIn">User info: {{ userInfo }}</span>
	<button @click="logOut">Logout</button>
	<RouterLink :to="{ name: 'auth', query: { mode: 'login' } }">Auth</RouterLink>
</template>

<style scoped></style>
