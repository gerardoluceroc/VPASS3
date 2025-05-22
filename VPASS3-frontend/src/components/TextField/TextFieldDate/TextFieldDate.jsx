import { TextField } from "@mui/material";

const TextFieldDate = ({
  label = "Seleccionar fecha", 
  width = "100%",
  onChange = () => {},
  error = false,
  helperText = "",
  name = ""
}) => {
  return (
    <TextField
      name={name}
      error={error}
      helperText={helperText}
      onChange={onChange}
      label={label}
      type="date"
      slotProps={{
          inputLabel: {
              shrink: true
          }
      }}
      sx={{
          width: {width},
          backgroundColor: '#fff',
          borderRadius: 2,
          boxShadow: 1,
          '& .MuiInputBase-root': {
          borderRadius: 2,
          },
      }}
    />
  )
}

export default TextFieldDate;