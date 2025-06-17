import { useTheme } from '@mui/material/styles';
import MobileStepper from '@mui/material/MobileStepper';
import Button from '@mui/material/Button';
import KeyboardArrowLeft from '@mui/icons-material/KeyboardArrowLeft';
import KeyboardArrowRight from '@mui/icons-material/KeyboardArrowRight';
import { useState } from 'react';

export default function ProgressStepperMui({
    width = "100%",
    titleNextButton = "Siguiente",
    titleBackButton = "Volver",
    ColorLineProgess = "#175676",
    ColorProgressBackground = "#E0E0E0",
    ColorNextButton = "#175676",
    ColorBackButton = "#175676",
    ColorDisabledButton = "#BDBDBD",
    WidthLineProgress = "80%",
    handleNext,
    handleBack,
    activeStep,
    backgroundColor = "white",
    steps = 6,
}) {
  const theme = useTheme();

  return (
    <MobileStepper
      variant="progress"
      steps={steps}
      position="static"
      activeStep={activeStep}
      sx={{ 
        width: width, 
        flexGrow: 1,
        '& .MuiLinearProgress-bar': {
            backgroundColor: ColorLineProgess,
        },
        '& .MuiMobileStepper-progress': {
            // width: WidthLineProgress,
            backgroundColor: ColorProgressBackground
        },
        backgroundColor: backgroundColor,
    }}
      nextButton={
        <Button 
          size="small" 
          onClick={handleNext} 
          disabled={activeStep === steps - 1}
          sx={{
            color: ColorNextButton,
            '&:hover': {
              backgroundColor: `${ColorNextButton}33`, // Con transparencia al hover
            },
            '&.Mui-disabled': {
              color: ColorDisabledButton,
            }
          }}
        >
          {titleNextButton}
          {theme.direction === 'rtl' ? (
            <KeyboardArrowLeft />
          ) : (
            <KeyboardArrowRight />
          )}
        </Button>
      }
      backButton={
        <Button 
          size="small" 
          onClick={handleBack} 
          disabled={activeStep === 0}
          sx={{
            color: ColorBackButton,
            '&:hover': {
              backgroundColor: `${ColorBackButton}33`, // Con transparencia al hover
            },
            '&.Mui-disabled': {
              color: ColorDisabledButton,
            }
          }}
        >
          {theme.direction === 'rtl' ? (
            <KeyboardArrowRight />
          ) : (
            <KeyboardArrowLeft />
          )}
          {titleBackButton}
        </Button>
      }
    />
  );
}