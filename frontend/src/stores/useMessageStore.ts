import { defineStore } from "pinia";
import { ref } from "vue";
import type { Message } from "../types";
import { useFetch } from "@vueuse/core";

export const useMessageStore = defineStore("messages", () => {
  const messages = ref<Record<string, Message[]>>({})
  const isFetching = ref(false)

  const getMessages = async (conversationId: string) => {
    if (messages.value[conversationId]) return

    isFetching.value = true;

    const {statusCode, data} = await useFetch<Message[]>(`/api/messages?conversationId=${conversationId}`).get();

    isFetching.value = false

    if (statusCode.value === 200 && data.value) {
      messages.value[conversationId] = data.value;
    }
  }

  const addMessage = async (conversationId: string, message: Message) => {
    messages.value[conversationId].push(message)
  }

  return {messages, isFetching, getMessages, addMessage }
})