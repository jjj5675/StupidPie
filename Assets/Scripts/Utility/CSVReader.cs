using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static bool Read(ref List<Phrases> phrases, string file)
    {
        foreach(var item in phrases)
        {
            item.key = "";
            item.phrases.Clear();
        }

        //데이터 로드
        TextAsset data = Resources.Load(file) as TextAsset;

        //행 별로 나누기
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1)
        {
            return false;
        }

        //1행의 헤더부분을 나누기 ex) 연출1, 2, 3...
        var header = Regex.Split(lines[0], SPLIT_RE);

        if(phrases.Count < header.Length * 0.5f)
        {
            Array.Resize(ref header, phrases.Count * 2);
        }

        //1행(헤더)를 제외한 나머지 행들의 갯수 만큼 반복
        for (int i = 1; i < lines.Length; i++)
        {
            // 선택한 행을 쉼표기준으로 나누기 정상적으론 헤더 갯수만큼 
            var valuse = Regex.Split(lines[i], SPLIT_RE);

            if(valuse.Length == 0 || valuse[0] == "")
            {
                continue;
            }

            //헤더를 순회 Name 갯수는 제외
            for(int k = 0; k< header.Length && k < valuse.Length; k += 2)
            {
                int objectIndex = Convert.ToInt32(k * 0.5f);

                if(valuse[k] == "")
                {
                    continue;
                }

                // 키 값을 헤더값으로 초기화
                if(phrases[objectIndex].key != header[k])
                {
                    phrases[objectIndex].key = header[k];
                }

                string value = valuse[k];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

                if (phrases[objectIndex].key == header[k])
                {
                    Phrase phrase = new Phrase(valuse[k + 1], value);
                    phrases[objectIndex].phrases.Add(phrase);
                }
            }
        }

        return true;
    }


    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);

            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                //헤더를 key, value를 넣기 헤더 갯수만큼 넣어질것
                entry[header[j]] = finalvalue;
            }
            //검색을 마친 행을 리스트에 넣기
            list.Add(entry);
        }
        return list;
    }
}