import { useFetch } from "@vueuse/core";
import { defineStore } from "pinia";
import { ref } from "vue";
import type { User } from "../types";

export const useUserStore = defineStore("user", () => {
	const isLoggedIn = ref<boolean>();
	const userInfo = ref<User>();

	const setInfo = async () => {
		if (userInfo.value && isLoggedIn) return;

		const { statusCode, data } = await useFetch("/api/user/info", {
			credentials: "include"
		})
			.get()
			.json();

		if (statusCode.value === 401) {
			isLoggedIn.value = false;
			return;
		}

		isLoggedIn.value = true;
		userInfo.value = data.value;
	};

	return { isLoggedIn, userInfo, setInfo };
}, {
	persist: {
		storage: sessionStorage,
	}
});
