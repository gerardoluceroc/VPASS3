import './ModalDescargarCsvEncomiendas.css';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  Button,
  Box,
  IconButton,
  Typography,
  Modal
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import { useFormik } from 'formik';
import TextFieldDate from '../../TextField/TextFieldDate/TextFieldDate';
import { useEffect } from 'react';
import RadioGroupMui from '../../RadioGroupMui/RadioGroupMui';
import ButtonTypeOne from '../../Buttons/ButtonTypeOne/ButtonTypeOne';
import { ValidationDescargarCsvEncomiendas } from './ValidationDescargarCsvEncomiendas';

const ModalDescargarCsvEncomiendas = ({ open, onClose }) => {

  const opcionesTiposDeDescarga = 
  [
    {
        id: 1,
        label: "Descargar todos"
    },
    {
        id: 2,
        label: "Descargar por rango de fechas"
    }

  ]
  const idOpcionDescargarTodos = 1;
  const idOpcionDescargarPorRangoFechas = 2;

    const formik = useFormik({
        initialValues: {
            fechaInicio: "",
            fechaFinal: "",
            idOpcionDescargaSeleccionada: idOpcionDescargarTodos,
        },
        validationSchema: ValidationDescargarCsvEncomiendas,
        onSubmit: async (values) => {
            console.log("submit descargar csv encomiendas")
            // setOpenLoadingRespuesta(true);
            // try {
            //     // Las fechas ya vienen en formato YYYY-MM-DD desde los inputs
            //     const result = await getVisitasPorRangoDeFechas(values.fechaInicio, values.fechaFinal);
                    
            //     if (result.success) {
            //         setOperacionExitosa(true);
            //         setMessageLoadingRespuesta("Reporte descargado con éxito")
            //     }
            //     else{
            //         setOperacionExitosa(false);
            //         setMessageLoadingRespuesta("Error al descargar el reporte");
            //     }
            // } catch (error) {
            //     setOperacionExitosa(false);
            //     setMessageLoadingRespuesta("Error al descargar el reporte");
            // }
        }
    });
    useEffect(() => {console.log("📌 - formik => ",formik.values)}, [formik.values]);

    const handleClose = () => {
        onClose();
    };

    useEffect(() => {
      if(open){
        formik.resetForm();
      }
    }, [open])
    

  return (
    <Modal
      open={open}
      onClose={handleClose}
    >
        <Box id="ContainerModalDescargarCsvEncomiendas">
            <Box id="HeaderModalDescargarCsvEncomiendas">
            <Typography>Descargar CSV de Encomiendas</Typography>
            <IconButton
                aria-label="close"
                onClick={handleClose}
                sx={{ color: 'black' }}
            >
                <CloseIcon sx={{ fontSize: '30px' }} />
            </IconButton>
            </Box>

            <RadioGroupMui
                    label="Seleccione su opción de descarga"
                    name="idOpcionDescargaSeleccionada"
                    listadoElementos={opcionesTiposDeDescarga}
                    keyListadoElementos="id"
                    atributoValue="id"
                    mostrarElemento={(option) => option.label}
                    handleChange={(e) => {
                        formik.setFieldValue('idOpcionDescargaSeleccionada', parseInt(e.target.value));
                    }}
                    elementoSeleccionado={formik.values.idOpcionDescargaSeleccionada}
                    helperText={formik.touched.idOpcionDescargaSeleccionada && formik.errors.idOpcionDescargaSeleccionada}
                    error={formik.touched.idOpcionDescargaSeleccionada && Boolean(formik.errors.idOpcionDescargaSeleccionada)}
                    row={false} // Para mostrar los radios en horizontal
                />

            <Box id="CuerpoModalDescargarCsvEncomiendas">
                {formik.values.idOpcionDescargaSeleccionada === idOpcionDescargarPorRangoFechas &&
                    <>
                        <Typography id="CabeceraRangoFechasDescargarRegistros">Rango de fechas</Typography>
                        <TextFieldDate
                            name="fechaInicio"
                            label="Fecha de inicio del reporte"
                            onChange={formik.handleChange}
                            error={formik.touched.fechaInicio && Boolean(formik.errors.fechaInicio)}
                            helperText={formik.touched.fechaInicio && formik.errors.fechaInicio}
                        />
                        <TextFieldDate
                            name="fechaFinal"
                            label="Fecha final del reporte"
                            onChange={formik.handleChange}
                            error={formik.touched.fechaFinal && Boolean(formik.errors.fechaFinal)}
                            helperText={formik.touched.fechaFinal && formik.errors.fechaFinal}
                        />
                    </>
                }
            </Box>

            <ButtonTypeOne
                defaultText="Descargar reporte"
                width="60%"
                handleClick={formik.handleSubmit}
                // loading={loading}
                loadingText="Descargando reporte..."
            />
        </Box>
    </Modal>
  );
};

export default ModalDescargarCsvEncomiendas;