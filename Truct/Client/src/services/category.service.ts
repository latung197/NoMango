import api from "../configs/api";

const path = "/categories";

export async function getCategories() {
   try {
  const res = await api.get(path);
  return res?.data.data;
  } catch (error) {
    console.log("Get categories error: ", error);
    throw new Error("Get categories error");
  }
}