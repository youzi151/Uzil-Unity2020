local this = {};

-- 從 C#層 取得
local flags = Json.decode(UZAPI.Flags.GetFlags_str());

-- 轉設置 並 確保 大寫
for idx = 1, #flags do
    local str = string.upper(flags[idx]);
    this[str] = true;
end

return this;