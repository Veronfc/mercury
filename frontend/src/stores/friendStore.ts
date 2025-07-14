export const useFriendStore = defineStore("friends", () => {
  const friends = ref<Friend[]>([]);
  const requestsSent = ref<FriendRequestSent[]>([]);
  const requestsReceived = ref<FriendRequestReceived[]>([]);
  const friendsFetching = ref(false);
  const requestsSentFetching = ref(false);
  const requestsReceivedFetching = ref(false);

  const getFriends = async () => {
    friendsFetching.value = true;

    const {statusCode, data} = await useFetch<Friend[]>("/api/friends", {credentials: "include"}).get().json<Friend[]>();

    friendsFetching.value = false;

    if (statusCode.value === 200 && data.value) {
      friends.value = data.value;
    }
  }

  const getRequestsSent = async () => {
    requestsSentFetching.value = true;

    const {statusCode, data} = await useFetch<FriendRequestSent[]>("/api/friends/sent", {credentials: "include"}).get().json<FriendRequestSent[]>();

    requestsSentFetching.value = false;

    if (statusCode.value === 200 && data.value) {
      requestsSent.value = data.value;
    }
  }

  const getRequestsReceived = async () => {
    requestsReceivedFetching.value = true;

    const {statusCode, data} = await useFetch<FriendRequestReceived[]>("/api/friends/received", {credentials: "include"}).get().json<FriendRequestReceived[]>();

    requestsReceivedFetching.value = false;

    if (statusCode.value === 200 && data.value) {
      requestsReceived.value = data.value;
    }
  }

  const acceptRequest = async (userId: string) => {
    const {statusCode, response } = await useFetch("/api/friends/accept").post({
      UserId: userId
    });

    if (statusCode.value !== 200) {
      return await response.value?.text();
    }

    getFriends();
  }

  const rejectRequest = async (userId: string) => {
    const {statusCode, response } = await useFetch("/api/friends/reject").post({
      UserId: userId
    });

    if (statusCode.value !== 200) {
      return await response.value?.text();
    }
  }

  const newRequest = async (userId: string) => {
    const { statusCode, response } = await useFetch("/api/friends/request").post({
      UserId: userId
    });

    if (statusCode.value !== 201) {
      return await response.value?.text();
    }

    getRequestsSent();
  }

  return {
    friends,
    requestsSent,
    requestsReceived,
    friendsFetching,
    requestsSentFetching,
    requestsReceivedFetching,
    getFriends,
    getRequestsSent,
    getRequestsReceived,
    acceptRequest,
    rejectRequest,
    newRequest
  }
}, {
  persist: {
    storage: sessionStorage
  }
})