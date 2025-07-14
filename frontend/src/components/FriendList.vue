<script setup lang="ts">
  const friendStore = useFriendStore();
  const { getFriends, getRequestsSent, getRequestsReceived } = friendStore;
  const { friends, requestsReceived, requestsSent, friendsFetching, requestsSentFetching, requestsReceivedFetching} = storeToRefs(friendStore);

  await getFriends(); 
  await getRequestsSent();
  await getRequestsReceived();

  onBeforeMount(async () => {
  })
</script>

<template>
  <div class="friend-list">
    <div v-if="!friendsFetching" class="friends">
      <div v-for="f in friends">
        {{ f }}
      </div>
    </div>
    <div v-else>Loading friends...</div>
    <hr>
    <div v-if="!requestsSentFetching" class="requests-sent">
      <div v-for="rs in requestsSent">
        {{ rs }}
      </div>
    </div>
    <div v-else>Loading requests sent...</div>
    <hr>
    <div v-if="!requestsReceivedFetching" class="requests-received">
      <div v-for="rr in requestsReceived">
        {{ rr }}
      </div>
    </div>
    <div v-else>Loading requests received...</div>
  </div>    
</template>

<style scoped>

</style>