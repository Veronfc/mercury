import { defineStore } from "pinia";
import { ref } from "vue";
import type { Conversation } from "../types";
import { useFetch } from "@vueuse/core";

export const useConversationStore = defineStore("conversations", () => {
	const conversations = ref<Conversation[]>([]);

	const getConversations = async () => {
		if (conversations.value) return

		const { statusCode, data } = await useFetch<Conversation[]>("/api/conversations", {
			credentials: "include"
		}).get().json();

		if (statusCode.value === 200) {
			conversations.value = data.value;
		}
	};

	const addConversation = (conversation: Conversation) => {
		conversations.value.push(conversation);
	};

	const updateConversation = (updatedConversation: Conversation) => {
		const index = conversations.value.findIndex(
			(c) => c.id === updatedConversation.id
		);
		
		if (index !== -1) {
			conversations.value[index] = updatedConversation;
		}
	};

	return {
		conversations,
		getConversations,
		addConversation,
		updateConversation
	};
});
