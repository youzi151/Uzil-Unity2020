
-- 註冊 當 全部模組 讀取完畢 (一次性)
EventBus.inst().once("modExample.onLoadModsEnd", "onLoadModsEnd", function ()
    print("modExample.onLoadModsEnd")
end);

print("modExample.onLoaded");