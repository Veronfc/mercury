<script setup lang="ts">
	import { storeToRefs } from "pinia";
	import { useMessageStore } from "../stores/messageStore";
	import { useRoute } from "vue-router";
	import { computed, watch } from "vue";
	import { useConversationStore } from "../stores/conversationStore";

	const route = useRoute();
	const { getConversation } = useConversationStore();
	const messageStore = useMessageStore();
	const { getMessages } = messageStore;
	const { isFetching, messages } = storeToRefs(messageStore);

	const conversationId = computed(() => route.params.id as string);

	const getDate = (dateStr: string) => {
		const date = new Date(dateStr)
		//TODO add today and yesterday check
		return new Intl.DateTimeFormat("en-GB", {
			weekday: "short",
			day: "2-digit",
			month: "short",
			year: "numeric"
		}).format(date)
	}

	const getTime = (dateStr: string) => {
		const date = new Date(dateStr);

		return new Intl.DateTimeFormat("en-GB", {
			hour: "2-digit",
			minute: "2-digit",
			hour12: false
		}).format(date);
	}

	watch(
		conversationId,
		(id) => {
			if (!getConversation(id)) {
				return;
			}

			getMessages(id);
		},
		{ immediate: true }
	);
</script>

<template>
	<div v-if="isFetching"></div>
	<div v-else>
		<div v-for="m in messages[conversationId]">
			<span>{{ m.content }}</span>
			<span>{{ m.senderId }}</span>
			<span>{{ getDate(m.sentAt) }}</span>
			<span>{{ getTime(m.sentAt) }}</span>
			<hr>
		</div>
	</div>
</template>

<style scoped></style>
