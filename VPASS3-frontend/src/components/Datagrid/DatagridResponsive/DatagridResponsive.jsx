// DatagridResponsive.jsx
// Source 1: https://github.com/mui/mui-x/issues/6460
// Source 2: https://codesandbox.io/p/sandbox/muidatatables-custom-toolbar-forked-j002q?file=%2Findex.js
import MUIDataTable from "mui-datatables";
import {
  Box,
  ThemeProvider,
  createTheme,
} from "@mui/material";
import { CacheProvider } from "@emotion/react";
import createCache from "@emotion/cache";

const muiCache = createCache({
  key: "mui-datatables",
  prepend: true,
});

// Tema específico para la tabla
const tableTheme = createTheme({
  components: {
    MUIDataTable: {
      styleOverrides: {
        root: {
          backgroundColor: '#eaeef7', // Color de fondo del resto de la tabla
          borderRadius: '8px', // Bordes redondeados de la tabla completa
          overflow: 'hidden',
        },
        paper: {
          boxShadow: 'none',
        },
      }
    },

    // Personalización de la cabecera de la tabla
    MUIDataTableHeadCell: {
      styleOverrides: {
        root: {
          // padding: '8px 32px',
          backgroundColor: '#175676', // Color de cabecera de la tabla
          color: '#FFFFFF', // Color del texto de la cabecera
          fontWeight: 'bold',
          '&:hover': {
            color: '#FFFFFF', // Color del texto de la cabecera al pasar el mouse
          }
        },
        sortAction: {
            alignItems: 'center',
        },
            sortActive: {
            color: '#FFFFFF', // Color del texto de la cabecera cuando está activo el ordenamiento
        },
        toolButton: {
            '&:hover': {
                backgroundColor: 'rgba(255, 255, 255, 0.1)', // color del fondo de una columna de cabecera al pasar el mouse
            }
        }
      }
    },
    
    MuiTableSortLabel: {
        styleOverrides: {
          icon: {
            color: '#FFFFFF !important', // Color del icono de flecha para ordenar
          }
        }
    },

    MUIDataTableBodyCell: {
      styleOverrides: {
        root: {
          padding: '8px 8px',
          '&.MuiDataTableBodyCell-selected': {
            backgroundColor: '#795548', // Fondo naranja claro para celdas seleccionadas
          }
        }
      }
    },

    MUIDataTableSelectCell: {
      styleOverrides: {
        root: {
          backgroundColor: '#175676', // Fondo para la columna de selección
        },
      }
    },
    
    MUIDataTableToolbarSelect: {
      styleOverrides: {
        root: {
          backgroundColor: ' #b3b6b7 ', // Barra de herramientas de selección naranja
          '& .MuiTypography-root': {
            color: '#FFFFFF', // Texto para los elementos de texto
            fontSize: "20px"
          },
          '& .MuiIconButton-root': {
            color: '#175676', // Iconos negros
          }
        }
      }
    },

    // Personalización de los checkboxes de la tabla en caso de que tenga el select de las filas activado
    MuiCheckbox: {
      styleOverrides: {
        root: {
          color: '#FFFFFF', // Color de los checkboxes no seleccionados (color de bordes)
          '&.Mui-checked': {
            color: '#FFFFFF', // Color de los checkboxes seleccionados
          }
        }
      }
    },

    // Personalización específica para el menú "Ver Columnas"
    MUIDataTableViewCol: {
      styleOverrides: {
        root: {
          backgroundColor: '#175676', // Fondo del menú
          padding: '16px',
          boxShadow: '0 4px 12px #175676',
          
          // Estilo del título
          '& .MuiTypography-root': {
            color: '#ecf0f1', // Color del texto del título
            fontWeight: 'bolder',
            fontSize: '1.2rem',
          },
          
          // Estilo de las etiquetas
          '& .MuiFormControlLabel-root': {
            marginLeft: '-8px',
            '& .MuiTypography-body1': {
              color: '#bdc3c7', // Color del texto de las opciones
              fontSize: '1.1rem'
            }
          },
          
          // Estilo de los checkboxes
          '& .MuiCheckbox-root': {
            color: '#ffffff', // Color del checkbox no seleccionado
            '&.Mui-checked': {
              color: '#FFFFFF', // Color del checkbox seleccionado
            },
          }
        }
      }
    },

    // Personalización específica para los botones del toolbar
    MUIDataTableToolbar: {
        styleOverrides: {
          root: {
            '& .MuiIconButton-root': {
              // Estilo normal de los botones
              color: '#175676', // Color de los iconos por defecto
              
              '&:hover': {
                backgroundColor: '#175676', // Color del circulo de fondo que envuelve los iconos al pasar el cursor
                color: '#FFFFFF', // Color del icono al pasar el cursor
              },
              
              '&.Mui-disabled': {
                color: '#b3b6b7', // Color cuando está deshabilitado
              }
            }
          }
        }
    },
  }
});

const DatagridResponsive = ({ 
  title = "Data Table", 
  data, 
  columns, 
  selectableRows = "none",
  downloadCsvButton = false,
  handleDownloadCsvButton = () => {},
  searchButton = true,
  viewColumnsButton = true,
  width = "100%",
  rowsPerPage = 10,
  rowsPerPageOptions = [5, 10, 20]
}) => {  

  const options = {
    search: searchButton, // true o false para mostrar el botón de búsqueda
    download: downloadCsvButton,
    onDownload: (buildHead, buildBody, columns, data) => {
        // console.log("Botón de descarga presionado");
        handleDownloadCsvButton();
        
        // console.log("Columnas:", columns);
        // console.log("Datos:", data);
        // console.log("buildBody:", buildBody);
        // console.log("buildHead:", buildHead);
        
        // Retornar false para cancelar la descarga automática
        return false;
    },
    print: false,
    viewColumns: viewColumnsButton, // true o false para mostrar el botón de ver columnas
    filter: false,
    filterType: "dropdown",
    responsive: "simple", //"vertical", "standard", "simple", "scroll", "scrollMaxHeight", "stacked",
    tableBodyHeight: "100%",
    tableBodyMaxHeight: "100%",
    rowsPerPage: rowsPerPage, // Número de filas por página
    rowsPerPageOptions: rowsPerPageOptions, // Opciones de filas por página
    selectableRows: selectableRows, // 'multiple', 'single' o 'none'
    pagination: true,
    sort: false,
    textLabels: {
      body: {
        noMatch: "Lo sentimos, no se encontraron registros",
      },
      toolbar: {
        search: "Buscar",
        downloadCsv: "Descargar CSV",
        print: "Imprimir",
        viewColumns: "Ver columnas",
        filterTable: "Filtrar tabla",
      },
      pagination: {
        rowsPerPage: "Filas por página:",
        displayRows: "de",
      },
      selectedRows: {
        text: "fila(s) seleccionada(s)",
        delete: "Eliminar",
        deleteAria: "Eliminar filas seleccionadas",
      },
      viewColumns: {
        title: "Columnas Visibles",
        titleAria: "Mostrar/Ocultar Columnas"
      }
    },
  };

  return (
    <Box sx={{width}}>
      <CacheProvider value={muiCache}>
        <ThemeProvider theme={tableTheme}>
          <MUIDataTable 
            title={title} 
            data={data} 
            columns={columns} 
            options={options} 
          />
        </ThemeProvider>
      </CacheProvider>
    </Box>
  );
};

export default DatagridResponsive;