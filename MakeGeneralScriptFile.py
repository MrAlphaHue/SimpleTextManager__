

import ConstManage as ConstManage
import glob
import openpyxl
import os

xlsxPath = ConstManage.xlsx_path + "/*.xlsx"
conststr_TableManager = """

//Warn : Don't change this code.
//Generated By MrHue.SimpleDataConverter


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO; 
using System;
using System.Linq;
using System.Security.Cryptography;

public class TableManager
{{
    //[RuntimeInitializeOnLoadMethod]
    //Must Be Static If You want "RuntimeInitializeOnLoadMethod"
    static void InIt()
    {{
        Debug.LogError("MrHue Table InIt");
        
{table_init}

    }}

    static Dictionary<string, string[][]> GetDicByFile(string tableName)
    {{
        Dictionary<string, string[][]> _outDic = new Dictionary<string, string[][]>();
        var text_asset = Resources.Load("BinaryData/" + tableName) as TextAsset;
        string getBytestring = System.Text.Encoding.UTF8.GetString(text_asset.bytes); //U2gi
        byte[] byte1 = Convert.FromBase64String(getBytestring);
        string final_str = System.Text.Encoding.UTF8.GetString(byte1);
        
        //SheetName / Sheet_Content / SheetName
        string[] strs = final_str.Split(new string[] {{"\t\t"}} ,StringSplitOptions.None);
        for (int i = 0; i < strs.Length - 1; i+=2)
        {{
            //
            string sheet_name = strs[i];
            
            //
            string[] strsSplit = strs[i+1].Split('\t');
            int _column = Array.FindIndex(strsSplit, val => val.Equals("id"));
            int _row = strsSplit.Length / _column;
            string[][] sheet_contents = new string[_row-2][];

            // int strsplindex;
            for (int idx_row = 2; idx_row < _row; idx_row++)
            {{
                sheet_contents[idx_row-2] = new string[_column];
                for (int idx_column = 0; idx_column < _column; idx_column++)
                {{
                    sheet_contents[idx_row-2][idx_column] = strsSplit[idx_row * _column + idx_column];
                }}
            }}
            _outDic.Add(sheet_name , sheet_contents);
        }}

        return _outDic;
    }}
}}
"""


conststr_tableinit =  """\t\t {table_name}.InIt(GetDicByFile("{table_name}"));\n"""

conststr_tablesheet_holder = """
using UnityEngine;

public class TableSheet_Holder
{
    public string SheetName = "";

    public TableSheet_Holder(string[][] strs)
    {
        //Debug.LogError(this.GetType().Name + " In It !!");
    }
}"""

def func_getBasefileName(file_name):
    base_filename = os.path.splitext(file_name)
    base_filename = os.path.split(base_filename[0])
    base_filename = base_filename[1]
    return base_filename

def Generate():
    strTableInIt = ""
    strMangerSc = ""
    for filename in glob.glob(xlsxPath):
        wb_obj = openpyxl.load_workbook(filename, read_only=False, data_only=True) #read_only = true, really loosy.
        base_filename = func_getBasefileName(filename)
        strTableInIt += conststr_tableinit.format(table_name = base_filename)
    
    strMangerSc += conststr_TableManager.format(table_init = strTableInIt)


    ######
    file = open(ConstManage.script_path + "/" + "TableManager.cs", "w+")
    file.write(strMangerSc)
    file_tablesheetHolder = open(ConstManage.script_path + "/" + "TableSheetHolder.cs", "w+")
    file_tablesheetHolder.write(conststr_tablesheet_holder)
    ######
    file_project = open(ConstManage.project_script_path + "/" + "TableManager.cs", "w+")
    file_project.write(strMangerSc)
    file_proeject_tablesheetHolder = open(ConstManage.project_script_path + "/" + "TableSheetHolder.cs", "w+")
    file_proeject_tablesheetHolder.write(conststr_tablesheet_holder)