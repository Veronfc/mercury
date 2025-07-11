<script setup lang="ts">
	import { storeToRefs } from "pinia";
	import { useUserStore } from "./stores/userStore";
	import { Icon } from "@iconify/vue";
	import { ref } from "vue";

	const { userInfo, isLoggedIn } = storeToRefs(useUserStore());
	const dismiss = ref(false);
</script>

<template>
	<div v-if="isLoggedIn && !userInfo?.displayName && !dismiss" class="notification">
		<span>You don't have a display name</span>
		<RouterLink @click="dismiss = !dismiss" :to="{ name: 'user' }" class="link"
			>Set it now</RouterLink
		>
		<button @click="dismiss = !dismiss">
			<Icon icon="fluent:dismiss-12-filled" />
		</button>
	</div>
	<Suspense>
		<RouterView />
	</Suspense>
</template>

<style scoped>
	@reference './style.css';

	.notification {
		@apply absolute top-4 border-2 border-red-500 rounded left-1/2 -translate-x-1/2 flex flex-col bg-white items-center p-2 gap-2;

		.link {
			@apply border p-2;
		}

		button {
			@apply cursor-pointer;
		}
	}
</style>
