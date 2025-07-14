<script setup lang="ts">
	const router = useRouter();
	const userStore = useUserStore();
	const { setInfo } = userStore;
	const { isLoggedIn, userInfo } = storeToRefs(userStore);

	// if (isLoggedIn.value) {
	// 	router.replace({ name: "conversations" });
	// }

	const logOut = async () => {
		await useFetch("api/user/logout", {
			credentials: "include"
		}).get();

		await setInfo();
		await disconnectSignalR();
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
	<hr>
</template>

<style scoped></style>
