// types/performance-charts.ts
export interface ProjectDataset {
  label: string;
  data: number[];
  backgroundColor: string;
  borderColor: string;
  borderWidth: number;
}

export interface ProjectOverviewData {
  labels: string[];
  datasets: ProjectDataset[];
}

export interface RequirementDataset {
  data: number[];
  backgroundColor: string[];
  borderColor: string[];
  borderWidth: number;
}

export interface RequirementsStatusData {
  labels: string[];
  datasets: RequirementDataset[];
}

export interface BugDataset {
  label: string;
  data: number[];
  fill: boolean;
  backgroundColor: string;
  borderColor: string;
  pointBackgroundColor: string;
  pointBorderColor: string;
  pointBorderWidth: number;
  pointRadius: number;
  tension: number;
}

export interface BugTrendData {
  labels: string[];
  datasets: BugDataset[];
}

export interface PerformanceChartsData {
  projectsData: ProjectOverviewData;
  requirementsData: RequirementsStatusData;
  bugsData: BugTrendData;
}

export interface ApiResponse<T> {
  status: boolean;
  data: T;
  message: string;
}
