import { toast } from "react-toastify";
import { getApiMessage } from "./apiMessage";

export function handleApiError(error: any) {
    const status = error?.response?.status;

    if (status === 401) {  
        window.location.href = "/login";
        return;
    }

    const message = getApiMessage(error);
    toast.error(message);
    throw error;
}