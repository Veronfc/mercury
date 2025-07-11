<script setup lang="ts">
import { useFetch } from '@vueuse/core';
import { useField, useForm } from 'vee-validate';
import { ref } from 'vue';
import { setDisplayNameSchema } from '../schemas/userSchema';
import { useUserStore } from '../stores/userStore';
import { useRouter } from 'vue-router';

	const success = ref(false);

	const { handleSubmit, errors } = useForm({
		validationSchema: setDisplayNameSchema
	});
	const { value: displayName } = useField("displayName");

	const { isFetching, statusCode, response, error, execute } = useFetch(
		"/api/user/displayname",
		{
			credentials: "include"
		},
		{
			immediate: false
		}
	).post(() => ({
		displayName: displayName.value,
	}));

	const errorMessage = ref();
	const { setInfo } = useUserStore();
	const router = useRouter();

	const setDisplayName = handleSubmit(async () => {
		await execute(false);

		if (error.value) {
			errorMessage.value = await response.value?.json();

			if ("DuplicateUserName" in errorMessage.value.errors) {
				errorMessage.value = "Email is already in use.";
			}
		}

		if (statusCode.value === 200) {
			success.value = true;

			setTimeout(async () => {
				router.push({ name: "home" });
			}, 2000);

      setInfo();
		}
	});
</script>

<template>
  <div v-if="isFetching">Loading...</div>
		<div v-else>
			<form @submit="setDisplayName">
				<label>
					Display name:
					<input name="displayname" v-model="displayName" type="text"/>
				</label>
				<span>{{ error.displayName }}</span>
				<button>Continue</button>
			</form>
  </div>    
</template>

<style scoped>

</style>