import { useFetch } from "@vueuse/core";
import { defineStore } from "pinia";
import { ref } from "vue";
import type { User } from "../types";

export const useUserStore = defineStore("user", () => {
	const isLoggedIn = ref<boolean>();
	const userInfo = ref<User>();

	const setLoggedIn = async (loggedIn?: boolean) => {
		if (loggedIn !== undefined) {
			isLoggedIn.value = loggedIn;

			return;
		}

		const { statusCode } = await useFetch("/api/user", {
			credentials: "include"
		}).get();

		isLoggedIn.value = statusCode.value !== 401;
	};

	const setInfo = async (info?: User) => {
		if (info) {
			userInfo.value = info;

			return;
		}

		const { statusCode, data } = await useFetch("/api/user/info", {
			credentials: "include"
		}).get().json();

		if (statusCode.value === 401) return;

		userInfo.value = data.value;
	};

	return { isLoggedIn, userInfo, setLoggedIn, setInfo };
});
