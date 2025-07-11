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

		await setInfo();
		router.go(0);
	};
</script>

<template>
	<div>This is the home view</div>
	<div>Logged in: {{ isLoggedIn }}</div>
	<div v-if="isLoggedIn">User info: {{ userInfo }}</div>
	<button @click="logOut" v-if="isLoggedIn">Logout</button>
	<br />
	<RouterLink :to="{ name: 'auth', query: { mode: 'login' } }">Auth</RouterLink>
	<br />
	<RouterLink :to="{ name: 'conversations' }">Conversations</RouterLink>
</template>

<style scoped></style>
