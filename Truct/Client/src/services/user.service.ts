import api from "../configs/api";
import { setUserInfoUtils } from "@/utils/authUtils";

const path = "/user";

export async function getUserInfo() {
    try {
        const res = await api.get(path + "/info");
        const data = res?.data.data;
        setUserInfoUtils(data);
        return data;
    } catch (error : any) {
        console.error("User-info error: ", error);
    }
}