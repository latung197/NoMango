import { Attribute } from '@/types/Product';
import React from 'react';

interface Props {
  attr: Attribute;
  attributeValues: Record<number, string | null>;
  onChange: (attrProductId: number, value: string | null) => void;
  isInvalid?: boolean;
}

const SelectTextAttribute: React.FC<Props> = ({ attr, attributeValues, onChange, isInvalid }) => {
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
            const selectedOption = attr.options?.find(
              (opt) => attributeValues[opt.productAttributeId] === opt.valueText
            );
            if (!selectedOption) return true;
            return selectedOption.productAttributeId === option.productAttributeId;
          })
          .map((option) => {
            const isSelected = attributeValues[option.productAttributeId] === option.valueText;

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
                      onChange(option.productAttributeId, option.valueText);
                    }
                  }}
                  onChange={() => {}}
                />
                <span>{option.valueText}</span>
              </label>
            );
          })}

        {attr.options.some((opt) => attributeValues[opt.productAttributeId] === opt.valueText) && (
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

export default SelectTextAttribute;