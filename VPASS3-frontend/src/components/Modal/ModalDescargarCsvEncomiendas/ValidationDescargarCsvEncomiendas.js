import * as Yup from 'yup';
import { esFechaMenorOIgual } from '../../../utils/funciones';

export const ValidationDescargarCsvEncomiendas = Yup.object().shape({
  fechaInicio: Yup.string()
    .required('El campo es requerido')
    .test('es-fecha-valida', 'Debe ser una fecha válida', value => !!value && !isNaN(Date.parse(value)))
    .when('fechaFinal', (fechaFinal, schema) => {
      return schema.test({
        name: 'es-menor-o-igual',
        message: 'Formato incorrecto, la fecha de inicio es posterior a la fecha final',
        test: function (fechaInicio) {
          if (!fechaInicio || !fechaFinal) return true; // No se valida si uno está vacío
          return esFechaMenorOIgual(fechaInicio, fechaFinal);
        }
      });
    }),

  fechaFinal: Yup.string()
    .required('El campo es requerido')
    .test('es-fecha-valida', 'Debe ser una fecha válida', value => !!value && !isNaN(Date.parse(value)))
});