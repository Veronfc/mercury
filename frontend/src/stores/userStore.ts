export const useUserStore = defineStore(
	"user",
	() => {
		const isLoggedIn = ref<boolean>(false);
		const userInfo = ref<User>();

		const setInfo = async () => {
			const { statusCode, data } = await useFetch("/api/user/info", {
				credentials: "include"
			})
				.get()
				.json();

			if (statusCode.value === 401) {
				isLoggedIn.value = false;
				userInfo.value = undefined;
				return;
			}

			isLoggedIn.value = true;
			userInfo.value = data.value;
		};

		return { isLoggedIn, userInfo, setInfo };
	},
	{
		persist: {
			storage: sessionStorage
		}
	}
);
