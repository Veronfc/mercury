import { useFetch } from "@vueuse/core";
import { defineStore } from "pinia";
import { ref } from "vue";
import { backendUrl } from "../main";

export const useUserStore = defineStore("user", () => {
	const isLoggedIn = ref(false);
	const userInfo = ref();

	const setLoggedIn = async (loggedIn?: boolean) => {
		if (loggedIn !== undefined) {
			isLoggedIn.value = loggedIn;

			return;
		}

		const { response } = await useFetch(`${backendUrl}/user`, {
			credentials: "include"
		}).get();

		isLoggedIn.value = response.value?.status !== 401;
	};

	const setInfo = async (info?: string) => {
		if (info !== undefined) {
			userInfo.value = info;

			return;
		}

		const { response } = await useFetch(`${backendUrl}/user/info`, {
			credentials: "include"
		}).get();

		if (response.value?.status === 401) return;

		userInfo.value = await response.value!.json();
	};

	setLoggedIn();
	setInfo();

	return { isLoggedIn, userInfo, setLoggedIn, setInfo };
});
