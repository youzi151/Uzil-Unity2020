local this = {};


--[[ 取得語言 ]] -- string
function this.getLanguage ()
    return UZAPI.I18n.GetLanguage();
end

--[[ 設置語言 ]] -- void
function this.setLanguage (lang)
    UZAPI.I18n.SetLanguage(lang);
end

--[[ 新增語言 ]] -- void
-- LangInfo { code:"en-us", title:"English"}
function this.addLanguage (langInfo)
    UZAPI.I18n.AddLanguage(Json.encode(langInfo));
end

--[[ 更新 ]] -- void
function this.update ()
    UZAPI.I18n.Update();
end

--[[ 設置 關鍵字 與 原始內容 ]] -- void
function this.keyword (key, raw)
    UZAPI.I18n.Keyword(key, raw);
end

--[[ 取得 該關鍵字的原始內容 ]] -- string
function this.raw (key)
    return UZAPI.I18n.Raw(key);
end

--[[ 移除關鍵字 ]] -- void
function this.remove (key)
    UZAPI.I18n.Remove(key);
end

--[[ 立刻代換 ]] -- string
function this.render (raw)
    return UZAPI.I18n.Render(raw);
end


--[[ 取得 可被代換內文 ]]
this.toReplace = {

    --[[ 棋局 自訂變數 ]]
    ["battleVar"] = function (val)
        return "<%battle:"..val.."%>";
    end

};



return this;