
import { ApiResponse } from "@/types/ApiResponse";
import api from "../configs/api";
import { toast } from "sonner";
import { getApiMessage } from "@/utils/apiMessage";

const path = "/auth";

export async function login(data: { email: string; password: string }) {
    try {
        const res = await api.post<ApiResponse<LoginResponse>>(path + "/login", data);
        return res?.data;
    } catch (error: any) {
        console.error("Login error: ", error);
        const message = getApiMessage(error);
        toast.error(message);
        throw error;
    }
}

export async function signUp(data: any) {
    try {
        const res = await api.post(path + "/sign-up", JSON.stringify(data));
        return res?.data;
    } catch (error : any) {
        console.error("Sign-up error: ", error);
        const message = getApiMessage(error);
        toast.error(message);
        throw error;
    }
}

export async function verifyAccount(data: any) {
    try {
        const res = await api.patch(path + "/verify-account", JSON.stringify(data));
        return res?.data;
    } catch (error : any) {
        console.error("Verify-account error: ", error);
        const message = getApiMessage(error);
        toast.error(message);
        throw error;

    }
}

export async function resendOtp(data: any) {
    try {
        const res = await api.post(path + "/resend-otp", JSON.stringify(data));
        return res?.data;
    } catch (error : any) {
        console.error("Resend-otp error: ", error);
        const message = getApiMessage(error);
        toast.error(message);
        throw error;
    }
}

export async function forgotPassword(data: any) {
    try {
        const res = await api.post(path + "/forgot-password", JSON.stringify(data));
        return res?.data;
    } catch (error : any) {
        console.error("Forgot password error: ", error);
        const message = getApiMessage(error);
        toast.error(message);
        throw error;
    }
}

export async function resetPassword(data: any) {
    try {
        const res = await api.patch(path + "/reset-password", JSON.stringify(data));
        return res?.data;
    } catch (error : any) {
        console.error("Reset password error: ", error);
        const message = getApiMessage(error);
        toast.error(message);
        throw error;
    }
}