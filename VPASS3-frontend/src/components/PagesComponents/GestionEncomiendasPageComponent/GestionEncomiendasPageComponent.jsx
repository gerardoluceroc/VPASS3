import { useEffect, useState } from "react";
import "./GestionEncomiendasPageComponent.css";
import UseEncomienda from "../../../hooks/useEncomienda/useEncomienda";
import useDepartamento from "../../../hooks/useDepartamento/useDepartamento";
import { cambiarFormatoHoraFecha, filtrarEncomiendasPendientes, filtrarEncomiendasRetiradas } from "../../../utils/funciones";
import { Box, Chip, Fade, IconButton } from "@mui/material";
import ButtonTypeOne from "../../Buttons/ButtonTypeOne/ButtonTypeOne";
import DatagridResponsive from "../../Datagrid/DatagridResponsive/DatagridResponsive";
import TableSkeleton from "../../Skeleton/TableSkeleton/TableSkeleton";
import InfoIcon from '@mui/icons-material/Info';
import TooltipTipoUno from "../../Tooltip/TooltipTipoUno/TooltipTipoUno";
import dayjs from "dayjs";
import ModalVerDetallesEncomienda from "../../Modal/ModalVerDetallesEncomienda/ModalVerDetallesEncomienda";
import ModalRegistarEncomienda from "../../Modal/ModalRegistrarEncomienda/ModalRegistrarEncomienda";
import MarkEmailReadIcon from '@mui/icons-material/MarkEmailRead';
import ModalRetirarEncomienda from "../../Modal/ModalRetirarEncomienda/ModalRetirarEncomienda";
import SelectMui from "../../Select/SelectMui/SelectMui";
import ModalDescargarCsvEncomiendas from "../../Modal/ModalDescargarCsvEncomiendas/ModalDescargarCsvEncomiendas";

const GestionEncomiendasPageComponent = () => {

    // Hook para obtener las encomiendas del usuario
    const { loading, response, responseStatus, getAllEncomiendas, encomiendas } = UseEncomienda();

    // Hook para obtener información de los departamentos del usuario
    const {getAllDepartamentos, departamentos} = useDepartamento();

    // Se obtienen las encomiendas desde el hook
    useEffect(() => {
      getAllEncomiendas();
      getAllDepartamentos();
    }, []);

    // Estado en donde se guardarán los datos de las encomiendas
    const [rowsOriginales, setRowsOriginales] = useState();
    useEffect(() => {
        if (!Array.isArray(encomiendas)) return;
    
        const encomiendasOrdenadasPorFecha = [...encomiendas].sort((a, b) =>
        dayjs(b.receivedAt).valueOf() - dayjs(a.receivedAt).valueOf()
        );
        setRowsOriginales(encomiendasOrdenadasPorFecha);
    }, [encomiendas]);

    // Estado y funciones para manejar las filas modificables de la tabla
    // Se utiliza para ordenar las encomiendas por fecha de llegada
    const [rowsModificables, setRowsModificables] = useState();
    useEffect(() => {
        // console.log("rows originales: ", JSON.stringify(rowsOriginales));
        if (!Array.isArray(rowsOriginales)) return;
    
        const encomiendasOrdenadasPorFecha = [...rowsOriginales].sort((a, b) =>
        dayjs(b.receivedAt).valueOf() - dayjs(a.receivedAt).valueOf()
        );
        setRowsModificables(encomiendasOrdenadasPorFecha);
    }, [rowsOriginales]);
    

    // Estado y funciones para manejar el modal de ver detalles de encomienda
    const [openModalVerDetallesEncomienda, setOpenModalVerDetallesEncomienda] = useState(false);
    const handleCloseModalVerDetallesEncomienda = () => setOpenModalVerDetallesEncomienda(false);
    const handleOpenModalVerDetallesEncomienda = (encomienda = {}) => {
      setOpenModalVerDetallesEncomienda(true);
      setEncomiendaSeleccionada(encomienda);
    };

    // Estado y funciones para manejar el modal de ver registrar encomienda
    const [openModalRegistrarEncomienda, setOpenModalRegistrarEncomienda] = useState(false);
    const handleCloseModalRegistrarEncomienda = () => setOpenModalRegistrarEncomienda(false);
    const handleOpenModalRegistrarEncomienda = () => setOpenModalRegistrarEncomienda(true);  

    // Estado y funciones para manejar el modal de retirar encomienda
    // Este modal se utiliza para marcar una encomienda como retirada
    const [openModalRetirarEncomienda, setOpenModalRetirarEncomienda] = useState(false);
    const handleCloseModalRetirarEncomienda = () => setOpenModalRetirarEncomienda(false);
    const handleOpenModalRetirarEncomienda = (encomienda = {}) => {
        setOpenModalRetirarEncomienda(true);
        setEncomiendaSeleccionada(encomienda);
    };

    // Departamento perteneciente a la encomienda seleccionada
    const [departamentoSeleccionado, setDepartamentoSeleccionado] = useState({});

    // Encomienda seleccionada para ver detalles
    const [encomiendaSeleccionada, setEncomiendaSeleccionada] = useState({});

    useEffect(() => {
        // En cuanto cambie la encomienda seleccionada, se cambia el departamento seleccionado
        const departamento = departamentos?.find((departamento) => departamento.id === encomiendaSeleccionada.idApartment);
        setDepartamentoSeleccionado(departamento || {});
    }, [encomiendaSeleccionada]);

    // Columnas de la tabla de encomiendas
    const columns = [
        "Destino",
        "Fecha de llegada",
        "Estado",
        "Acciones"
    ];

    // Filas de la tabla de encomiendas
    const data = rowsModificables?.map((encomienda) => {
        const {receivedAt, deliveredAt} = encomienda;
        const destino = departamentos?.find((departamento) => departamento.id === encomienda.idApartment) || "Desconocido";
        const {zoneName: zonaDestino, name: nombreDepartamento} = destino;
        return[
            `${zonaDestino} - Departamento ${nombreDepartamento}`,
            cambiarFormatoHoraFecha(receivedAt) || "Sin datos",
            deliveredAt ? <Chip label="Retirada" color="success" /> : <Chip label="Pendiente" color="error" />,
            <Box id="BoxAccionesTablaUltimosRegistros">
                <TooltipTipoUno titulo={"Ver detalles"} ubicacion={"right"}>
                <IconButton onClick={()=>{handleOpenModalVerDetallesEncomienda(encomienda)}}>
                    <InfoIcon id="BotonVerDetallesRegistro" fontSize="large" />
                </IconButton>
                </TooltipTipoUno>

                {deliveredAt ?
                    null
                    :
                    <TooltipTipoUno titulo={"Marcar como entregada"} ubicacion={"right"}>
                        <IconButton onClick={()=>{handleOpenModalRetirarEncomienda(encomienda)}}>
                            <MarkEmailReadIcon id="BotonVerDetallesRegistro" fontSize="large" />
                        </IconButton>
                    </TooltipTipoUno>  
                }
            </Box>
        ]
    })

    // Variables y funciones para manejar el filtrado de las encomiendas
    const idOpcionTodasSelectVerTablaEncomiendas = 1;
    const idOpcionPendientesSelectVerTablaEncomiendas = 2;
    const idOpcionRetiradasSelectVerTablaEncomiendas = 3;
    const opcionesSelectVerTablaEncomiendas = 
    [
        {
            id: 1,
            nombre: "Todas"
        },
        {
            id: 2,
            nombre: "Pendientes"
        },
        {
            id: 3,
            nombre: "Retiradas"
        },
    ]
    const [idOpcionSeleccionadaSelectVerTablaEncomiendas, setIdOpcionSeleccionadaSelectVerTablaEncomiendas] = useState(idOpcionTodasSelectVerTablaEncomiendas);

    // Funcion para filtrar encomiendas por "todas", "pendientes" y "retiradas"
    const handleChangeSelectVerTablaEncomiendas = (idOpcion) => {
        setIdOpcionSeleccionadaSelectVerTablaEncomiendas(idOpcion);

        if(idOpcion === idOpcionTodasSelectVerTablaEncomiendas ){
            setRowsModificables(rowsOriginales);
        }
        else if(idOpcion === idOpcionRetiradasSelectVerTablaEncomiendas){
            const encomiendasRetiradas = filtrarEncomiendasRetiradas(rowsOriginales);
            setRowsModificables(encomiendasRetiradas);
        }
        else if(idOpcion === idOpcionPendientesSelectVerTablaEncomiendas){
            const encomiendasPendientes = filtrarEncomiendasPendientes(rowsOriginales);
            setRowsModificables(encomiendasPendientes);
        }
    }

    // Estado y funciones para abrir el modal para descargar el csv con las encomiendas de la tabla
    const [openModal, setOpenModal] = useState(false);
    const handleOpenModalDescargarCsv = () =>{setOpenModal(true)}
    const handleCloseModalDescargarCsv = ()=>{setOpenModal(false)}

    return (
        <Box id="ContainerGestionEncomiendasPageComponent">
            <Box id= "HeaderGestionEncomiendasPageComponent">
                <Box id="ItemHeaderGestionEncomiendasPage">
                    <ButtonTypeOne
                        defaultText="Registrar nueva encomienda"
                        handleClick={()=>{handleOpenModalRegistrarEncomienda()}}
                    />
                </Box>
                <Box id="ItemHeaderGestionEncomiendasPage">
                    <SelectMui
                        label = "Ver"
                        width={"100%"}
                        listadoElementos={opcionesSelectVerTablaEncomiendas || []}
                        keyListadoElementos={"id"}
                        mostrarElemento={(option)=> option["nombre"]}
                        handleChange = {(e)=>{handleChangeSelectVerTablaEncomiendas(e.target.value)}}
                        elementoSeleccionado = {idOpcionSeleccionadaSelectVerTablaEncomiendas}
                        atributoValue={"id"}
                        fontSize="18px"
                        backgroundColor="#175676"
                        color="white"
                        borderColor="white"
                        focusBorderColor="white"
                        labelColorNormal="white"
                    />
                </Box>
            </Box>
            <Fade in={!(!Array.isArray(rowsOriginales))} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
                <div>
                    <DatagridResponsive 
                        title="Encomiendas" 
                        columns={columns} 
                        data={data} 
                        selectableRows="none" 
                        downloadCsvButton={true} 
                        handleDownloadCsvButton={handleOpenModalDescargarCsv} />
                    <ModalVerDetallesEncomienda
                        open={openModalVerDetallesEncomienda}
                        onClose={handleCloseModalVerDetallesEncomienda}
                        encomiendaSeleccionada={encomiendaSeleccionada}
                        departamentoSeleccionado={departamentoSeleccionado}
                    />
                    <ModalRegistarEncomienda
                        open={openModalRegistrarEncomienda}
                        onClose={handleCloseModalRegistrarEncomienda}
                        setRows={setRowsOriginales}
                        departamentos={departamentos}
                    />

                    <ModalRetirarEncomienda
                        open={openModalRetirarEncomienda}
                        onClose={handleCloseModalRetirarEncomienda}
                        setRows={setRowsOriginales}
                        encomiendaSeleccionada={encomiendaSeleccionada}
                        setEncomiendaSeleccionada={setEncomiendaSeleccionada}
                        departamentoSeleccionado={departamentoSeleccionado}
                    />

                    <ModalDescargarCsvEncomiendas
                        open={openModal}
                        onClose={handleCloseModalDescargarCsv}
                    />
                </div>
            </Fade>

            <Fade in={!Array.isArray(rowsOriginales)} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
                <div>
                    <TableSkeleton columnCount={3} rowCount={7} />
                </div>
            </Fade>
        </Box>
    )
}

export default GestionEncomiendasPageComponent;