import './ModalDescargarCsvEncomiendas.css';
import {
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
import UseEncomienda from '../../../hooks/useEncomienda/useEncomienda';
import { idOpcionDescargarPorRangoFechas, idOpcionDescargarTodos, opcionesTiposDeDescarga } from './constantesDescargarCsvEncomiendas';

const ModalDescargarCsvEncomiendas = ({ open, onClose }) => {

    const {loading, exportarEncomiendasPorRangoDeFechas, exportarTodasLasEncomiendas} = UseEncomienda();
        const formik = useFormik({
            initialValues: {
                fechaInicio: "",
                fechaFinal: "",
                idOpcionDescargaSeleccionada: idOpcionDescargarTodos,
            },
            validationSchema: ValidationDescargarCsvEncomiendas,
            onSubmit: async (values) => {

                if(values.idOpcionDescargaSeleccionada === idOpcionDescargarTodos){
                    const respuesta = await exportarTodasLasEncomiendas();
                }

                else if(values.idOpcionDescargaSeleccionada === idOpcionDescargarPorRangoFechas){
                    const respuesta = await exportarEncomiendasPorRangoDeFechas(values.fechaInicio, values.fechaFinal);
                }
            }
    });
    // useEffect(() => {console.log("📌 - formik values => ",formik.values)}, [formik.values]);
    // useEffect(() => {console.log("📌 - formik errors => ",formik.errors)}, [formik.errors]);

    const handleClose = () => {
        onClose();
    };

    useEffect(() => {
      if(open){
        formik.resetForm();
      }
    }, [open])

    // Si se cambia a la opcion para descargar todas las encomiendas
    // Se debe resetear fechaInicio y fechaFinal para que no queden guardados valores anteriores
    useEffect(() => {
      if(formik.values.idOpcionDescargaSeleccionada === idOpcionDescargarTodos){
        formik.setFieldValue("fechaInicio", "");
        formik.setFieldValue("fechaFinal", "");
      }
    }, [formik.values.idOpcionDescargaSeleccionada])
    
    
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
                loading={loading}
                loadingText="Descargando reporte..."
            />
        </Box>
    </Modal>
  );
};

export default ModalDescargarCsvEncomiendas;