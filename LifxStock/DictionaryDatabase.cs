/*
 * Copyright (C) 2010 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.Text;
using Android.Util;
using LifxStock.Core.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace LifxStock
{
    public class DictionaryDatabase
    {
        static String TAG = "StocksDatabase";
        //The columns we'll include in the dictionary table
        public static readonly String KEY_WORD = SearchManager.SuggestColumnText1;
        public static readonly String KEY_DEFINITION = SearchManager.SuggestColumnText2;
        //static String DATABASE_NAME = "stocksSearch";
        static String FTS_VIRTUAL_TABLE = "FTSstocks";
        //static int DATABASE_VERSION = 1;
        DictionaryOpenHelper databaseOpenHelper;
        static Dictionary<string, string> mColumnMap = BuildColumnMap();

        public DictionaryDatabase(Context context)
        {
            databaseOpenHelper = new DictionaryOpenHelper(context);
        }

        static Dictionary<string, string> BuildColumnMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add(KEY_WORD, KEY_WORD);
            map.Add(KEY_DEFINITION, KEY_DEFINITION);
            map.Add(Android.Provider.BaseColumns.Id, "rowid AS " +
                Android.Provider.BaseColumns.Id);
            map.Add(SearchManager.SuggestColumnIntentDataId, "rowid AS " +
                SearchManager.SuggestColumnIntentDataId);
            map.Add(SearchManager.SuggestColumnShortcutId, "rowid AS " +
                SearchManager.SuggestColumnShortcutId);
            return map;
        }

        public Android.Database.ICursor GetWord(String rowId, String[] columns)
        {
            String selection = "rowid = ?";
            String[] selectionArgs = new String[] { rowId };

            return Query(selection, selectionArgs, columns);
        }

        public Android.Database.ICursor GetWordMatches(String query, String[] columns)
        {
            String[] selectionArgs = new String[] { query };

            return QueryCustom(selectionArgs, columns);
        }

        Android.Database.ICursor Query(String selection, String[] selectionArgs, String[] columns)
        {
            var builder = new SQLiteQueryBuilder();
            builder.Tables = FTS_VIRTUAL_TABLE;
            builder.SetProjectionMap(mColumnMap);

            var cursor = builder.Query(databaseOpenHelper.WritableDatabase,
                                        columns, selection, selectionArgs, null, null, null);

            if (cursor == null)
            {
                return null;
            }
            else if (!cursor.MoveToFirst())
            {
                cursor.Close();
                return null;
            }
            return cursor;
        }

        Android.Database.ICursor QueryCustom(String[] selectionArgs, String[] columns)
        {
            var builder = new SQLiteQueryBuilder();
            builder.Tables = FTS_VIRTUAL_TABLE;
            builder.SetProjectionMap(mColumnMap);

            var selectionValue = selectionArgs[0].Replace("*", "");
            var cursor = databaseOpenHelper.WritableDatabase
                .RawQuery(@"SELECT rowid AS _id, suggest_text_1, suggest_text_2, rowid AS suggest_intent_data_id 
                FROM FTSstocks 
                WHERE suggest_text_1 LIKE '%" + selectionValue + "%' OR suggest_text_2 LIKE '%" + selectionValue + "%'", null);

            if (cursor == null)
            {
                return null;
            }
            else if (!cursor.MoveToFirst())
            {
                cursor.Close();
                return null;
            }
            return cursor;
        }

        class DictionaryOpenHelper : SQLiteOpenHelper
        {
            private static string DATABASE_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            private static string DATABASE_FILE_NAME = "StocksSearch.sqlite";
            private static int VERSION = 1;
            private Context context = null;
            static SQLiteDatabase _objSQLiteDatabase = null;

            public DictionaryOpenHelper(Context _context) : base(_context, DATABASE_FILE_NAME, null, VERSION)
            {
                this.context = _context;
            }

            #region implemented abstract members of SQLiteOpenHelper

            public override void OnCreate(SQLiteDatabase _database)
            {
                CreateSQLiteDatabase();
            }

            public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
            {

            }

            #endregion

            public override SQLiteDatabase WritableDatabase
            {
                get
                {
                    SQLiteDatabase _objBaseSQLiteDatabase = base.WritableDatabase;
                    if (_objSQLiteDatabase == null)
                    {
                        CreateSQLiteDatabase();
                    }
                    return _objSQLiteDatabase;
                }
            }

            public SQLiteDatabase CreateSQLiteDatabase()
            {
                string _strSQLitePathOnDevice = GetSQLitePathOnDevice();
                Stream _streamSQLite = null;
                FileStream _streamWrite = null;
                Boolean isSQLiteInitialized = false;

                try
                {
                    if (File.Exists(_strSQLitePathOnDevice))
                    {
                        isSQLiteInitialized = true;
                    }
                    else
                    {
                        _streamSQLite = context.Resources.OpenRawResource(Resource.Raw.StocksSearch);
                        _streamWrite = new FileStream(_strSQLitePathOnDevice, FileMode.OpenOrCreate, FileAccess.Write);
                        if (_streamSQLite != null && _streamWrite != null)
                        {
                            if (CopySQLiteOnDevice(_streamSQLite, _streamWrite))
                            {
                                isSQLiteInitialized = true;
                            }
                        }
                    }
                    if (isSQLiteInitialized)
                    {
                        _objSQLiteDatabase = SQLiteDatabase.OpenDatabase(_strSQLitePathOnDevice, null, DatabaseOpenFlags.OpenReadonly);
                    }
                }
                catch (Exception _exception)
                {
                    MethodBase _currentMethod = MethodInfo.GetCurrentMethod();
                    Console.WriteLine(String.Format("CLASS : {0}; METHOD : {1}; EXCEPTION : {2}"
                        , _currentMethod.DeclaringType.FullName
                        , _currentMethod.Name
                        , _exception.Message));
                }
                return _objSQLiteDatabase;
            }

            private string GetSQLitePathOnDevice()
            {
                string _strSQLitePathOnDevice = string.Empty;
                try
                {
                    _strSQLitePathOnDevice = Path.Combine(DATABASE_DIRECTORY, DATABASE_FILE_NAME);
                }
                catch (Exception _exception)
                {
                    MethodBase _currentMethod = MethodInfo.GetCurrentMethod();
                    Console.WriteLine(String.Format("CLASS : {0}; METHOD : {1}; EXCEPTION : {2}"
                        , _currentMethod.DeclaringType.FullName
                        , _currentMethod.Name
                        , _exception.Message));
                }
                return _strSQLitePathOnDevice;
            }

            private bool CopySQLiteOnDevice(Stream _streamSQLite, Stream _streamWrite)
            {
                bool _isSuccess = false;
                int _length = 256;
                Byte[] _buffer = new Byte[_length];
                try
                {
                    int _bytesRead = _streamSQLite.Read(_buffer, 0, _length);
                    while (_bytesRead > 0)
                    {
                        _streamWrite.Write(_buffer, 0, _bytesRead);
                        _bytesRead = _streamSQLite.Read(_buffer, 0, _length);
                    }
                    _isSuccess = true;
                }
                catch (Exception _exception)
                {
                    MethodBase _currentMethod = MethodInfo.GetCurrentMethod();
                    Console.WriteLine(String.Format("CLASS : {0}; METHOD : {1}; EXCEPTION : {2}"
                        , _currentMethod.DeclaringType.FullName
                        , _currentMethod.Name
                        , _exception.Message));
                }
                finally
                {
                    _streamSQLite.Close();
                    _streamWrite.Close();
                }
                return _isSuccess;
            }
        }

        /*class DictionaryOpenHelper : SQLiteOpenHelper
        {
            Context helperContext;
            SQLiteDatabase database;
            static String FTS_TABLE_CREATE =
                    "CREATE VIRTUAL TABLE " + FTS_VIRTUAL_TABLE +
                " USING fts3 (" +
                KEY_WORD + ", " +
                KEY_DEFINITION + ");";

            public DictionaryOpenHelper(Context context) : base(context, DATABASE_NAME, null, DATABASE_VERSION)
            {
                helperContext = context;
            }

            public override async void OnCreate(SQLiteDatabase db)
            {
                database = db;
                database.ExecSQL(FTS_TABLE_CREATE);
                await LoadStocksAsync();
            }

            public async Task LoadStocksAsync()
            {
                Log.Debug(TAG, "Loading stocks...");

                var resources = helperContext.Resources;
                var inputStream = resources.OpenRawResource(Resource.Raw.stocks);

                using (var reader = new System.IO.StreamReader(inputStream))
                {
                    try
                    {
                        String line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            var strings = TextUtils.Split(line, " - ");
                                
                            AddWord(strings[0].Trim(), strings[1].Trim());
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }

                Log.Debug(TAG, "DONE loading stocks.");
            }

            public long AddWord(String word, String definition)
            {
                var initialValues = new ContentValues();
                initialValues.Put(KEY_WORD, word);
                initialValues.Put(KEY_DEFINITION, definition);

                return database.Insert(FTS_VIRTUAL_TABLE, null, initialValues);
            }

            public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
            {
                Log.Warn(TAG, "Upgrading database from version " + oldVersion + " to "
                    + newVersion + ", which will destroy all old data");
                db.ExecSQL("DROP TABLE IF EXISTS " + FTS_VIRTUAL_TABLE);
                OnCreate(db);
            }
        }*/
    }
}

