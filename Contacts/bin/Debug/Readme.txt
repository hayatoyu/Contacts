﻿1. 通訊錄資料紀錄於 Contracts.csv，若有更動請直接更新csv檔案，並儲存成 UTF-8 格式。
   並請維持資料最後一行為<EOF>標籤。

2. 請將 Contracts.csv 檔案與執行檔放置於同一資料夾。

3. 支援四種查詢方式(均依單位名稱、職級排序)

	(a) 以人名關鍵字查詢 - 可輸入部分名或姓氏，將列出符合關鍵字的人選分機

	(b) 以單位查詢 - 分成單位名稱關鍵字及表列所有單位後再從中選擇

		(1)單位關鍵字 - 可輸入部分單位名稱關鍵字，或列出符合的單位名稱下之所有人員
		
		(2)表列 - 表列出研發處所有單位代號名稱，並依輸入代號再表列該單位下所有人員

	(c) 以分機查詢 - 輸入分機可查該分機人員資料

	(d) 以特殊需求查詢 - 有的人員專責某些系統，如Notes，選這個選項的話會列出專責的人員資訊

4. 支援重取資料 - 如 csv 檔案有變更，或資料庫更改，可重新取得資料。

5. 支援畫面清除 - 輸入 cls 可清除畫面上資訊。

6. 支援切換 Data Source - App Config文件中有個 Data Source 的tag，設為1為從本地 csv 檔案找資料，設為2則為

			  連線至 SQL Server (不過你們可能連不到我的資料庫)