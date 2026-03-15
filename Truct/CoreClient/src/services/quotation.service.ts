
import { ApiResponse } from "@/types/ApiResponse";
import api from "../configs/api";
import { QuotationFromCartRequest, QuotationHistoryRequest, QuotationHistoryResponse, QuotationRequest, QuotationResponse } from "@/types/Quotation";
import { getApiMessage } from "@/utils/apiMessage";
import { toast } from "sonner";

const path = "/quotation";

export async function quotation(data: QuotationRequest) {
    try {
        const res = await api.post<ApiResponse<QuotationResponse>>(path, data);
        return res?.data;
    } catch (error : any) {
        console.error("Quotation error: ", error);
        const message = getApiMessage(error);
        toast.error(message);
        throw error;
    }
}

export async function quotationDownload(id: number): Promise<void> {
    try {
        const res = await api.post(`${path}/${id}/download`, null, {
            responseType: 'blob',
        });
        const now = new Date();
        const pad = (n: number) => n.toString().padStart(2, '0');
        const timestamp = `${now.getFullYear()}${pad(now.getMonth() + 1)}${pad(now.getDate())}${pad(now.getHours())}${pad(now.getMinutes())}${pad(now.getSeconds())}`;

        let filename = `見積書_${timestamp}.pdf`;

        const blob = new Blob([res.data], { type: "application/pdf" });
        const url = URL.createObjectURL(blob);

        const link = document.createElement("a");
        link.href = url;
        link.download = filename;
        link.click();

        URL.revokeObjectURL(url);
    } catch (error : any) {
        console.error("Quotation download error: ", error);
        const message = getApiMessage(error);
        toast.error(message);
        throw error;
    }
}

export async function quotationFromCart(data: QuotationFromCartRequest) {
    try {
        const res = await api.post<ApiResponse<QuotationResponse>>(path + "/cart", data);
        return res?.data;
    } catch (error : any) {
        console.error("Quotation from cart error: ", error);
        const message = getApiMessage(error);
        toast.error(message);
        throw error;
    }
}

export async function quotationHistory(request: QuotationHistoryRequest) {
    try {
        const res = await api.get<ApiResponse<QuotationHistoryResponse[]>>(path + "/history", { params: request});
        return res?.data;
    } catch (error : any) {
        console.error("Quotation from cart error: ", error);
        const message = getApiMessage(error);
        toast.error(message);
        throw error;
    }
}

export async function getQuotationDetail(id: number) {
    try {
        const res = await api.get<ApiResponse<QuotationResponse>>(path + "/" + id);
        return res?.data;
    } catch (error : any) {
        console.error("Quotation detail error: ", error);
        const message = getApiMessage(error);
        toast.error(message);
        throw error;
    }
}