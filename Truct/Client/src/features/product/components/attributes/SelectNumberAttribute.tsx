import { Attribute } from '@/types/Product';
import React from 'react';

interface Props {
  attr: Attribute;
  attributeValues: Record<number, number | null>;
  onChange: (attrProductId: number, value: number | null) => void;
  isInvalid?: boolean;
}

const SelectNumberAttribute: React.FC<Props> = ({ attr, attributeValues, onChange, isInvalid }) => {
  if (!attr.options) return null;
  let errorMessage = null;
  if (isInvalid && !errorMessage) {
    errorMessage = "値を選択してください";
  }

  return (
    <div>
        <div className={`rounded p-4 space-y-2 text-xs border ${isInvalid ? 'border-red-500' : 'border-gray-300'}`}>
          {(attr.options ?? [])
            .filter((option) => {
              const selectedProductAttrId = attr.options?.find(
                (opt) => attributeValues[opt.productAttributeId] !== null && attributeValues[opt.productAttributeId] !== undefined
              )?.productAttributeId;

              if (!selectedProductAttrId) return true;
              return selectedProductAttrId === option.productAttributeId;
            })
            .map((option) => {
              const isSelected = attributeValues[option.productAttributeId] === option.valueNumber;

              return (
                <label key={option.productAttributeId} className="flex items-center space-x-2">
                  <input
                    type="radio"
                    name={`attribute-group-${attr.attributeId}`}
                    checked={isSelected}
                    onClick={() => {
                      if (isSelected) {
                        onChange(option.productAttributeId, null);
                      } else {
                        onChange(option.productAttributeId, option.valueNumber);
                      }
                    }}
                    onChange={() => {}}
                  />
                  <span>{option.valueNumber}</span>
                </label>
              );
            })}
            
          {attr.options.some((opt) => attributeValues[opt.productAttributeId] !== null && attributeValues[opt.productAttributeId] !== undefined) && (
            <div className="text-right mt-2">
              <button
                onClick={() => {
                  attr.options?.forEach((opt) => {
                    onChange(opt.productAttributeId, null);
                  });
                }}
                className="text-blue-600 text-xs underline hover:text-blue-800"
              >
                選択を解除する
              </button>
            </div>
          )}
        </div>
        {errorMessage && (
          <div className="text-red-500 text-[10px] mt-1 ml-1">{errorMessage}</div>
        )}
    </div>
    
  );
};
export default SelectNumberAttribute;
