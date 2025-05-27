// ValidationCrearZona.js
import * as Yup from 'yup';

const ValidationCrearZona = Yup.object().shape({
  nombreZona: Yup.string()
    .required('El campo es requerido')
    .trim()
    .min(1, 'El nombre de la zona no puede estar vacío'),
});

export default ValidationCrearZona;
