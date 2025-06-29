<script setup lang="ts">
import { storeToRefs } from 'pinia';
import { useMessageStore } from '../stores/useMessageStore';
import { useRoute } from 'vue-router';

  const route = useRoute()
  const messageStore = useMessageStore()
  const { getMessages } = messageStore
  const { isFetching, messages } = storeToRefs(messageStore)

  const conversationId = route.params.id as string

  getMessages(conversationId);
</script>

<template>
  <div v-if="isFetching">

  </div>
  <div v-else>
    <div v-for="m in messages[conversationId]">
      <span>{{ m.content }}</span>
      <span>{{ m.senderId }}</span>
      <span>{{ m.sentAt }}</span>
    </div>
  </div>    
</template>

<style scoped>

</style>