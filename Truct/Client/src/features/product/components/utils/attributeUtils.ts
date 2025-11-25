import { Attribute, AttributeOption, AttributeType } from "@/types/Product";


export const getAttrProductId = (attr: Attribute, option?: AttributeOption): number => {
    const selectTypes: AttributeType[] = [
        AttributeType.SELECT_NUMBER,
        AttributeType.SELECT_TEXT,
    ];
    if (selectTypes.includes(attr.type)) {
        if (!option) throw new Error('Option is required for SELECT types');
        return option.productAttributeId;
    }
    if (attr.productAttributeId == null) throw new Error('attrProductId is missing in attribute');
    return attr.productAttributeId;
};
