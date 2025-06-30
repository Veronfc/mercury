<script setup lang="ts">
	import { useFetch } from "@vueuse/core";
	import { useUserStore } from "../stores/userStore";
	import { useRouter } from "vue-router";
import { storeToRefs } from "pinia";
import { onMounted } from "vue";

	const userStore = useUserStore()
	const {isLoggedIn, userInfo} = storeToRefs(userStore);
	const {setInfo} = userStore
	const router = useRouter();

	const {statusCode, data, execute} = useFetch("/api/user", {
		credentials: "include"
	}, {
		immediate: false
	}).get().json()
	
	const logOut = async () => {
		await useFetch("api/user/logout", {
			credentials: "include"
		}).get();
		
		setInfo();
		router.go(0);
	};
	
	onMounted(async () => {
		await execute()

		if (statusCode.value === 200) {
			console.log(data.value)
		}
	})
</script>

<template>
	<div>This is the home view</div>
	<span>Logged in: {{ isLoggedIn }}</span>
	<span v-if="isLoggedIn">User info: {{ userInfo }}</span>
	<button @click="logOut">Logout</button>
	<RouterLink :to="{ name: 'auth', query: { mode: 'login' } }">Auth</RouterLink>
</template>

<style scoped></style>
