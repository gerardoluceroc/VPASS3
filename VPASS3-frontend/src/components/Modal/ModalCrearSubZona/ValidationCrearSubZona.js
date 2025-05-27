import * as Yup from 'yup';

const ValidationCrearSubZona = Yup.object().shape({
  nombreSubZona: Yup.string()
    .required('El campo es requerido')
    .trim()
    .min(1, 'El nombre de la subzona no puede estar vacío'),
});

export default ValidationCrearSubZona;