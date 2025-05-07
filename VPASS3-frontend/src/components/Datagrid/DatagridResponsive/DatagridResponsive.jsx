// DatagridResponsive.jsx
// Source 1: https://github.com/mui/mui-x/issues/6460
// Source 2: https://codesandbox.io/p/sandbox/muidatatables-custom-toolbar-forked-j002q?file=%2Findex.js

import React, { useState } from "react";
import MUIDataTable from "mui-datatables";
import {
  ThemeProvider,
  createTheme,
  InputLabel,
  MenuItem,
  FormControl,
  Select,
} from "@mui/material";
import { CacheProvider } from "@emotion/react";
import createCache from "@emotion/cache";

const muiCache = createCache({
  key: "mui-datatables",
  prepend: true,
});

const DatagridResponsive = ({ title = "Data Table", data, columns }) => {
  const [responsive, setResponsive] = useState("simple");
  const [tableBodyHeight, setTableBodyHeight] = useState("100%");
  const [tableBodyMaxHeight, setTableBodyMaxHeight] = useState("");
  const [searchBtn, setSearchBtn] = useState(true);
  const [downloadBtn, setDownloadBtn] = useState(true);
  const [printBtn, setPrintBtn] = useState(true);
  const [viewColumnBtn, setViewColumnBtn] = useState(true);
  const [filterBtn, setFilterBtn] = useState(true);

  const options = {
    search: searchBtn,
    download: downloadBtn,
    print: printBtn,
    viewColumns: viewColumnBtn,
    filter: filterBtn,
    filterType: "dropdown",
    responsive,
    tableBodyHeight,
    tableBodyMaxHeight,
  };

//   div style={{ display: "flex", flexWrap: "wrap" }}>
//           {renderSelect("Responsive Option", responsive, setResponsive, [
//             "vertical",
//             "standard",
//             "simple",
//             "scroll",
//             "scrollMaxHeight",
//             "stacked",
//           ])}
//           {renderSelect("Table Body Height", tableBodyHeight, setTableBodyHeight, [
//             "",
//             "400px",
//             "800px",
//             "100%",
//           ])}
//           {renderSelect("Max Table Body Height", tableBodyMaxHeight, setTableBodyMaxHeight, [
//             "",
//             "400px",
//             "800px",
//             "100%",
//           ])}
//           {renderSelect("Search Button", searchBtn, setSearchBtn, [true, false])}
//           {renderSelect("Download Button", downloadBtn, setDownloadBtn, [true, false])}
//           {renderSelect("Print Button", printBtn, setPrintBtn, [true, false])}
//           {renderSelect("View Column Button", viewColumnBtn, setViewColumnBtn, [true, false])}
//           {renderSelect("Filter Button", filterBtn, setFilterBtn, [true, false])}
//         </div>

//   const renderSelect = (label, value, setValue, options) => (
//     <FormControl>
//       <InputLabel>{label}</InputLabel>
//       <Select
//         value={value}
//         style={{ width: "200px", marginBottom: "10px", marginRight: 10 }}
//         onChange={(e) => setValue(e.target.value)}
//       >
//         {options.map((opt, idx) => (
//           <MenuItem key={idx} value={opt}>
//             {String(opt)}
//           </MenuItem>
//         ))}
//       </Select>
//     </FormControl>
//   );

  return (
    <CacheProvider value={muiCache}>
      <ThemeProvider theme={createTheme()}>
        <MUIDataTable title={title} data={data} columns={columns} options={options} />
      </ThemeProvider>
    </CacheProvider>
  );
};

export default DatagridResponsive;