import { AttributeType, ProductMetaResponse, SkuAttrResponse } from "@/types/Product";
import React from "react";

interface SkuSelectorProps {
  shippingAttribute: ProductMetaResponse;
  selectedSku: number | null;
  setSelectedSku: (id: number | null) => void;
  isSkuInvalid: boolean;
  setIsSkuInvalid: (val: boolean) => void;
  setAttributeValues: React.Dispatch<React.SetStateAction<Record<number, string | number | null>>>;
  invalidAttrIds: number[];
  setInvalidAttrIds: React.Dispatch<React.SetStateAction<number[]>>;
}

const SkuSelector: React.FC<SkuSelectorProps> = ({
  shippingAttribute,
  selectedSku,
  setSelectedSku,
  isSkuInvalid,
  setIsSkuInvalid,
  setAttributeValues,
  invalidAttrIds,
  setInvalidAttrIds
}) => {
  if (!shippingAttribute?.skus || shippingAttribute.skus.length === 0) return null;

  const handleSelect = (sku: SkuAttrResponse) => {
    const isSame = selectedSku === sku.skuId;

    if (isSame) {
        setSelectedSku(null);
        setAttributeValues((prev) => {
            const cleanedValues = { ...prev };
            Object.keys(sku.attributeIdMap || {}).forEach((attributeIdStr) => {
            const attributeId = Number(attributeIdStr);
            const attr = shippingAttribute.attributes.find((a) => a.attributeId === attributeId);
            const relatedIds = attr?.options?.map((opt) => opt.productAttributeId) || [];
            relatedIds.forEach((id) => delete cleanedValues[id]);
            });
            return cleanedValues;
        });
        setInvalidAttrIds((prev) => {
            const cleaned = [...prev];
            Object.keys(sku.attributeIdMap || {}).forEach((attributeIdStr) => {
                const attributeId = Number(attributeIdStr);
                const index = cleaned.indexOf(attributeId);
                if (index !== -1) cleaned.splice(index, 1);
            });
            return cleaned;
        });
        return;
    }

    setSelectedSku(sku.skuId);
    setIsSkuInvalid(false);

    setAttributeValues((prev) => {
      const cleanedValues = { ...prev };
      Object.keys(sku.attributeIdMap || {}).forEach((attributeIdStr) => {
        const attributeId = Number(attributeIdStr);
        const attr = shippingAttribute.attributes.find((a) => a.attributeId === attributeId);
        const relatedIds = attr?.options?.map((opt) => opt.productAttributeId) || [];
        relatedIds.forEach((id) => delete cleanedValues[id]);
      });

      const updatedValues: Record<number, string | number | null> = { ...cleanedValues };
      Object.entries(sku.attributeIdMap || {}).forEach(([attributeIdStr, productAttrId]) => {
        const attributeId = Number(attributeIdStr);
        const attr = shippingAttribute.attributes.find((a) => a.attributeId === attributeId);
        const matchedOption = attr?.options?.find((opt) => opt.productAttributeId === productAttrId);

        if (matchedOption) {
          updatedValues[productAttrId] =
            attr?.type === AttributeType.SELECT_TEXT ? matchedOption.valueText ?? null : matchedOption.valueNumber ?? null;
        }
      });

      return updatedValues;
    });
    setInvalidAttrIds((prev) => {
        const cleaned = [...prev];
        Object.keys(sku.attributeIdMap || {}).forEach((attributeIdStr) => {
            const attributeId = Number(attributeIdStr);
            const index = cleaned.indexOf(attributeId);
            if (index !== -1) cleaned.splice(index, 1);
        });
        return cleaned;
    });
  };

  return (
    <div className="mt-4">
        <h3 className="font-semibold mb-2 text-sm">タイプ</h3>
        <div
            className={`border rounded p-4 space-y-2 text-xs ${
            isSkuInvalid ? "border-red-500" : "border-gray-300"
            }`}
        >
            {shippingAttribute.skus
            .filter((sku) => selectedSku === null || selectedSku === sku.skuId)
            .map((sku) => (
                <label key={sku.skuId} className="flex items-center space-x-2">
                <input
                    type="radio"
                    name="sku"
                    className="mr-2"
                    checked={selectedSku === sku.skuId}
                    onClick={() => handleSelect(sku)}
                />
                {sku.sku}
                </label>
            ))}
            {selectedSku !== null && (
            <div className="text-right mt-2">
                <button
                onClick={() => setSelectedSku(null)}
                className="text-blue-600 text-xs underline hover:text-blue-800"
                >
                選択を解除する
                </button>
            </div>
            )}
        </div>
        {isSkuInvalid && (
            <div className="text-red-500 text-[10px] mt-1 ml-1">{"値を選択してください"}</div>
        )}
    </div>
  );
};

export default SkuSelector;