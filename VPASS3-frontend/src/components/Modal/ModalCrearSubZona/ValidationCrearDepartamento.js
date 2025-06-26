import * as Yup from 'yup';

const ValidationCrearDepartamento = Yup.object().shape({
  nombreDepartamento: Yup.string()
    .required('El campo es requerido')
    .trim()
    .min(1, 'El nombre del departamento no puede estar vacío'),
});

export default ValidationCrearDepartamento;