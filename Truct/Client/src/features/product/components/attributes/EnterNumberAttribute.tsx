import { Attribute } from '@/types/Product';

interface Props {
  attr: Attribute;
  value: number | null;
  onChange: (value: number | null) => void;
  isInvalid?: boolean;
  reportError: (hasError: boolean) => void;
}

const EnterNumberAttribute: React.FC<Props> = ({ attr, value, onChange, isInvalid, reportError }) => {
  const isTooSmall = attr.minValue !== null && value !== null && value < attr.minValue;
  const isTooLarge = attr.maxValue !== null && value !== null && value > attr.maxValue;

  let errorMessage =
    isTooSmall
      ? `最小値は ${attr.minValue} です。`
      : isTooLarge
      ? `最大値は ${attr.maxValue} です。`
      : null;

  if (isInvalid && !errorMessage) {
    errorMessage = "値を入力してください";
  }

  const showError = errorMessage || isInvalid;
  return (
    <div className="text-xs">
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-2">
          <input
            type="number"
            min={attr.minValue ?? undefined}
            max={attr.maxValue ?? undefined}
            value={value ?? ''}
            onChange={(e) => {
              const raw = e.target.value;
              const num = raw ? Number(raw) : null;

              const tooSmall = attr.minValue !== null && num !== null && num < attr.minValue;
              const tooLarge = attr.maxValue !== null && num !== null && num > attr.maxValue;
              const hasError = tooSmall || tooLarge;

              reportError?.(hasError);
              onChange(num);
            }}
            onWheel={(e) => (e.target as HTMLInputElement).blur()}
            className={`border rounded px-2 pr-0 py-1 w-24 focus:outline-none focus:ring-1 ${
                showError ? 'border-red-500 focus:ring-red-500' : 'border-gray-300 focus:ring-black'
              }`}

          />
          {/* {attr.unit && <span className="text-gray-500">{attr.unit}</span>} */}
        </div>

        {(attr.minValue !== null || attr.maxValue !== null) && (
          <span className="text-gray-400 text-[10px] ml-4 whitespace-nowrap">
            {attr.minValue !== null && `最小値: ${attr.minValue}`}{" "}
            {attr.maxValue !== null && `最大値: ${attr.maxValue}`}
          </span>
        )}
      </div>

      {errorMessage && (
        <div className="text-red-500 text-[10px] mt-1 ml-1">{errorMessage}</div>
      )}
    </div>
  );
};

export default EnterNumberAttribute;