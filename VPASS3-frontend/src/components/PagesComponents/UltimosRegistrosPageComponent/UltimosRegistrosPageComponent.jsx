import { Box, Fade, IconButton } from "@mui/material";
import DatagridResponsive from "../../Datagrid/DatagridResponsive/DatagridResponsive";
import "./UltimosRegistrosPageComponent.css";
import useVisita from "../../../hooks/useVisita/useVisita";
import { useEffect, useState } from "react";
import { cambiarFormatoHoraFecha } from "../../../utils/funciones";
import InfoIcon from '@mui/icons-material/Info';
import ModalVerDetallesRegistros from "../../Modal/ModalVerDetallesRegistro/ModalVerDetallesRegistro";
import TooltipTipoUno from "../../Tooltip/TooltipTipoUno/TooltipTipoUno";
import dayjs from "dayjs";
import TableSkeleton from "../../Skeleton/TableSkeleton/TableSkeleton";

const UltimosRegistrosPageComponent = () => {
    const { getAllVisitas, visitas } = useVisita();

    // Información para gestionar el modal de detalles de registro
    const [visitaSeleccionada, setVisitaSeleccionada] = useState({});
    const [rows, setRows] = useState();
    const [openModalDetallesVerRegistro, setOpenModalDetallesVerRegistro] = useState(false);
    const handleOpenModalDetallesVerRegistro = (visita = {}) => {
      setOpenModalDetallesVerRegistro(true);
      setVisitaSeleccionada(visita);
    };
    const handleCloseModalDetallesVerRegistro = () => setOpenModalDetallesVerRegistro(false);

    useEffect(() => {
      getAllVisitas();
    }, []);

    // Cuando carguen las visitas desde el servidor, se proceden a ordenar por fecha.
    useEffect(() => {
      if (!Array.isArray(visitas)) return;
    
      const visitasOrdenadasPorFecha = [...visitas].sort((a, b) =>
        dayjs(b.entryDate).valueOf() - dayjs(a.entryDate).valueOf()
      );
      setRows(visitasOrdenadasPorFecha);
    }, [visitas]);
  
    const columns = ["Nombre", "Rut", "Destino", "Sentido", "Fecha", "Acciones"];

    const data = rows?.map((visita) => {
      const { person, zone, zoneSection, direction, entryDate: horaEntrada } = visita;
      const { names = "", lastNames = "", identificationNumber = "" } = person || {};
      const { name: nombreZona = "" } = zone || {};
      const { name: nombreSubzona = "" } = zoneSection || {};
      const { visitDirection: sentido = "" } = direction || {};
      const columnaAcciones = 
      <Box id="BoxAccionesTablaUltimosRegistros">
        <TooltipTipoUno titulo={"Ver detalles"} ubicacion={"right"}>
          <IconButton onClick={()=>handleOpenModalDetallesVerRegistro(visita)}>
            <InfoIcon id="BotonVerDetallesRegistro" fontSize="large" />
          </IconButton>
        </TooltipTipoUno>
      </Box>;

    return [
      `${names} ${lastNames}`.trim(),
      identificationNumber,
      `${nombreZona} ${nombreSubzona}`,
      sentido,
      cambiarFormatoHoraFecha(horaEntrada),
      columnaAcciones || "Sin datos"
    ];
  });

  return (
    <Box id="ContainerUltimosRegistrosPageComponent">
      <Fade in={!(!Array.isArray(rows))} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
          <div>
              <DatagridResponsive title="Últimos Registros" columns={columns} data={data} selectableRows="none" downloadCsvButton={false} />
              <ModalVerDetallesRegistros
                open={openModalDetallesVerRegistro}
                onClose={handleCloseModalDetallesVerRegistro}
                visitaSeleccionada={visitaSeleccionada}
              />
          </div>
      </Fade>

      <Fade in={!Array.isArray(rows)} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
          <div>
              <TableSkeleton columnCount={3} rowCount={7} />
          </div>
      </Fade>
    </Box>
  );

};

export default UltimosRegistrosPageComponent;