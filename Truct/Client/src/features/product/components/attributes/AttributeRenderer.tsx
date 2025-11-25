import { Attribute, AttributeType } from '@/types/Product';
import React from 'react';
import EnterNumberAttribute from './EnterNumberAttribute';
import SelectNumberAttribute from './SelectNumberAttribute';
import SelectTextAttribute from './SelectTextAttribute';
import FixNumberAttribute from './FixNumberAttribute';
import FixTextAttribute from './FixTextAttribute';

interface Props {
  attr: Attribute;
  attributeValues: Record<number, string | number | null>;
  onChange: (attrProductId: number, value: string | number | null) => void;
  isInvalid?: boolean;
  reportError: (hasError: boolean) => void;
}

const AttributeRenderer: React.FC<Props> = ({ attr, attributeValues, onChange, isInvalid, reportError }) => {
  switch (attr.type) {
    case AttributeType.ENTER_NUMBER:
      return (
        <EnterNumberAttribute
          attr={attr}
          value={attributeValues[attr.productAttributeId!] as number | null}
          onChange={(val) => onChange(attr.productAttributeId!, val)}
          isInvalid={isInvalid}
          reportError={reportError}
        />
      );

    case AttributeType.SELECT_NUMBER:
      return (
        <SelectNumberAttribute
          attr={attr}
          attributeValues={attributeValues as Record<number, number | null>}
          onChange={onChange}
          isInvalid={isInvalid}
        />
      );

    case AttributeType.SELECT_TEXT:
      return (
        <SelectTextAttribute
          attr={attr}
          attributeValues={attributeValues as Record<number, string | null>}
          onChange={onChange}
          isInvalid={isInvalid}
        />
      );

    case AttributeType.FIX_NUMBER:
      return <FixNumberAttribute attr={attr} />;

    case AttributeType.FIX_TEXT:
      return <FixTextAttribute attr={attr} />;

    default:
      return null;
  }
};

export default AttributeRenderer;