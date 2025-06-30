<script setup lang="ts">
	import { useForm, useField } from "vee-validate";
	import { registerSchema } from "../schemas/authSchema";
	import { useFetch } from "@vueuse/core";
	import { ref } from "vue";
	import { useRouter } from "vue-router";
	import { useUserStore } from "../stores/userStore";

	const success = ref(false);

	const { handleSubmit, errors } = useForm({
		validationSchema: registerSchema
	});
	const { value: email } = useField("email");
	const { value: password } = useField("password");

	const { isFetching, response, error, execute } = useFetch(
		"/api/user/register",
		{
			credentials: "include"
		},
		{
			immediate: false
		}
	).post(() => ({
		email: email.value,
		password: password.value
	}));

	const errorMessage = ref();
	const userStore = useUserStore();
	const router = useRouter();

	const register = handleSubmit(async () => {
		await execute(false);

		if (error.value) {
			errorMessage.value = await response.value?.json();

			if ("DuplicateUserName" in errorMessage.value.errors) {
				errorMessage.value = "Email is already in use.";
			}
		}

		if (response.value?.status === 200) {
			success.value = true;

			setTimeout(async () => {
				router.push({ name: "home" });
			}, 2000);

			await useFetch("/api/user/login?useCookies=true", {
				credentials: "include"
			}).post(() => ({ email: email.value, password: password.value }));

			userStore.setInfo();
		}
	});
</script>

<template>
	<div v-if="success">Registered successfully</div>
	<div v-else>
		<div v-if="isFetching">Loading...</div>
		<div v-else>
			Register Form
			<form @submit="register">
				<input name="email" v-model="email" type="email" />
				<span>{{ errors.email }}</span>
				<input name="password" v-model="password" type="text" />
				<span>{{ errors.password }}</span>
				<button>Register</button>
			</form>
			<span v-if="error">{{ errorMessage }}</span>
			<RouterLink :to="{ name: 'auth', query: { mode: 'login' } }"
				>Login</RouterLink
			>
		</div>
	</div>
</template>

<style scoped></style>
