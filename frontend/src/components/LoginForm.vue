<script setup lang="ts">
	import { useForm, useField } from "vee-validate";
	import { loginSchema } from "../schemas/authSchema";
	import { useFetch } from "@vueuse/core";
	import { useRouter } from "vue-router";
	import { useUserStore } from "../stores/userStore";

	const { handleSubmit, errors } = useForm({
		validationSchema: loginSchema
	});
	const { value: email } = useField("email");
	const { value: password } = useField("password");

	const { isFetching, statusCode, error, execute } = useFetch(
		"/api/user/login?useCookies=true",
		{
			credentials: "include"
		},
		{
			immediate: false
		}
	)
		.post(() => ({ email: email.value, password: password.value }))
		.json();

	const userStore = useUserStore();
	const router = useRouter();

	const logIn = handleSubmit(async () => {
		await execute(false);

		if (statusCode.value === 200) {
			userStore.setInfo();
			router.push({ name: "home" });
		}
	});
</script>

<template>
	<div v-if="isFetching">Loading...</div>
	<div v-else>
		Login Form
		<form @submit="logIn">
			<input name="email" v-model="email" type="email" />
			<span>{{ errors.email }}</span>
			<input name="password" v-model="password" type="text" />
			<button>Login</button>
		</form>
		<span v-if="error">Login failure</span>
		<RouterLink :to="{ name: 'auth', query: { mode: 'register' } }"
			>Register</RouterLink
		>
	</div>
</template>

<style scoped></style>
